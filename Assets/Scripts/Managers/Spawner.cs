// Debug checks for Start()
#define SPAWNER_DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using BugCatcher.Utils;
using System;
using BugCatcher.Extensions;
using UnityEngine.Diagnostics;

// TODO: This is copy-pasted code from a previous project. We need to change it accordingly.
public class Spawner : MonoBehaviour
{
    private enum SpawnPosMethod { ExactPos, InsideBounds }

    [Header("Object settings")]
    [SerializeField] GameObject original;
    [SerializeField] Transform parent;
    [SerializeField] SpawnPosMethod method;
    [SerializeField] Vector2 exactPos;
    [SerializeField] Bounds  bounds;

    [Header("Spawn time & chance")]
    [Min(0f)][SerializeField]       float waitInitSecs = 0f;
    [Min(0f)][SerializeField]       float waitLoopSecs = 5f;
    [Range(0f, 1f)][SerializeField] float spawnChance = 1.0f;

    [Header("Events")]
    [SerializeField] public UnityEvent beforeSpawn = new();
    [SerializeField] public UnityEvent<GameObject> afterSpawn = new();
    public Func<bool> proceedSpawnCheck = () => true; // To change via another script


    private string HeaderStr { get => $"[Spawner(name: {gameObject.name}, id: {GetInstanceID()})] - "; }

    void Start()
    {
#if SPAWNER_DEBUG
        if ( original.IsMissingOrNone() )
        {
            throw new UnassignedReferenceException( HeaderStr + "Original object instance to spawn not set" );
        }

        if ( original.TryGetComponent<Spawner>( out _ ) )
        {
            Debug.LogWarning(
                HeaderStr +
                "Original game object instance contains an Spawner component, " +
                "which will more likely result in an endless loop of spawners " +
                "if they're not disabled after instantiating."
            );
        }

        if ( ReferenceEquals( this.gameObject, original ) )
        {
            Debug.LogWarning(
                HeaderStr +
                "This spawner will copy its own gameObject, which will more " +
                "likely result in an endless loop of spawners if they're not " +
                "disabled after instantiating."
           );
        }
#endif

        StartCoroutine( SpawnLoop() );
    }

    /// <summary>
    /// Main spawn loop. Can wait before starting and spawn with chance.
    /// </summary>
    /// <returns>WaitForSeconds</returns>
    /// <exception cref="System.ArgumentException">Unknown SpawnPosMethod</exception>
    IEnumerator SpawnLoop()
    {
        if ( waitInitSecs > 0f )
            yield return new WaitForSeconds( waitInitSecs );

        // UnityEngine.Random isn't thread-safe, apparently
        System.Random rng = new(Thread.CurrentThread.ManagedThreadId);

#if SPAWNER_DEBUG
        Debug.Log( HeaderStr + "Initiating SpawnLoop" );
#endif

        while ( true )
        {
#if SPAWNER_DEBUG
            Debug.Log( HeaderStr + "SpawnLoop called" );
#endif

            // position could be moved into a member Func and assigned
            // this wait on Start(), but let's leave it like this for now.
            Vector2 position = method switch
            {
                SpawnPosMethod.ExactPos
                    => exactPos,

                SpawnPosMethod.InsideBounds
                    => BC_Random.InsideBounds(rng, bounds),

                _ => throw new System.ArgumentException("Unknown SpawnPosMethod")
            };

            if ( proceedSpawnCheck()
            && ( spawnChance == 1f || rng.NextFloat( 0f, 1f ) <= spawnChance ) )
            {
                beforeSpawn.Invoke();

                var go = Instantiate(original, position, Quaternion.identity, parent);
                go.SetActive( true );

                afterSpawn.Invoke( go );
            }

            yield return new WaitForSeconds( waitLoopSecs );
        }
    }

    public void SetSpawnParent( Transform parent ) => this.parent = parent;
}