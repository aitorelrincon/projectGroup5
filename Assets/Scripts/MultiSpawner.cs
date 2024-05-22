// Debug checks for Start()
#define SPAWNER_DEBUG

using System;
using System.Linq;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

using BugCatcher.Utils;
using BugCatcher.Extensions;
using BugCatcher.Utils.ObjectPooling;

using PoolArgs = PoolSetup.PoolSetupArgs;

[DisallowMultipleComponent]
public class MultiSpawner : MonoBehaviour
{
    #region Nested types
    public enum SpawnPosMethod { ExactPos, InsideBounds, InsideSpherePoints }

    [Serializable]
    public class PrefabConfig
    {
        [SerializeField]        PoolArgs _poolArgs;
        [Range(0f, 1f)] public  float    proportion;
        public PoolArgs  poolArgs { get => _poolArgs; }

        [SerializeField, HideInInspector] bool _serialized = false;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            if ( !_serialized )
            {
                proportion = 1f;
                _serialized = true;
            }
        }
    }
    #endregion

    #region Spawner config
    // [Header("Prefabs config")]
    [SerializeField] PrefabConfig[] prefabsConfig;

    [Header("Spawn config")]
    [SerializeField] Transform      spawnedParent    = null;
    [SerializeField] Transform      pooledParent     = null;
    [SerializeField] SpawnPosMethod method;
    [SerializeField] Vector3        exactPos;
    [SerializeField] Bounds         bounds;
    [SerializeField] float          sphereRadius;
    [SerializeField] bool           northHemisphere  = true;
    [SerializeField] bool           childrenAsPoints = true;
    [SerializeField] Transform[]    points;

    [Header("Spawn time & chance")]
    [SerializeField, Min(0f)]        float waitInitSecs = 0f;
    [SerializeField, Min(0f)]        float waitLoopSecs = 5f;
    [SerializeField, Range( 0f, 1f)] float spawnChance  = 1.0f;

    [Header("Events")]
    [SerializeField] public UnityEvent              beforeSpawn = new();
    [SerializeField] public UnityEvent<GameObject>  afterSpawn  = new();
    public Func<bool> proceedSpawnCheck = () => true; // To change via another script
    #endregion

    #region Instance variables
    Pool[]  _pools;
    Func<System.Random, Vector3> _NextPosition;
    #endregion

    #region Properties
    private string HeaderStr =>  $"[MultiSpawner(name: {gameObject.name}, id: {GetInstanceID()})] - ";
    public int spawnedCount { get => spawnedParent.childCount; }
    #endregion

    void Start()
    {
        if ( spawnedParent.IsMissingOrNone() )
        {
            var go = new GameObject( "MS_SpawnedParent" );
            spawnedParent = go.transform;
        }

        if ( pooledParent .IsMissingOrNone() )
        {
            var go = new GameObject( "MS_PooledParent" );
            pooledParent = go.transform;
        }

#if SPAWNER_DEBUG
        if ( prefabsConfig.Length == 0 )
            Debug.LogWarning(
                HeaderStr +
                "No prefabs found to instantiate"
            );
#endif

        _pools = new Pool[prefabsConfig.Length];
        for ( int i = 0; i < prefabsConfig.Length; ++i )
        {
#if SPAWNER_DEBUG
            if ( prefabsConfig[i].poolArgs.prefab.IsMissingOrNone() )
                throw new UnassignedReferenceException( HeaderStr + "Prefab to spawn not set at index: " + i );

            if ( ReferenceEquals( gameObject, prefabsConfig[i].poolArgs.prefab ) )
                Debug.LogWarning(
                    HeaderStr +
                    "This spawner will copy its own gameObject, which will more " +
                    "likely result in an endless loop of spawners if they're not " +
                    "disabled after instantiating."
                );
#endif

            _pools[i] = Pool.GetAndFill( prefabsConfig[i].poolArgs, pooledParent );
        }

        if ( method == SpawnPosMethod.InsideSpherePoints )
        {
#if SPAWNER_DEBUG
            if ( childrenAsPoints
            &&   transform.childCount == 0 )
                Debug.LogWarning(
                    HeaderStr +
                    "This spawner is set to use it's children as spheric spawn points " +
                    "but has no children"
                );
            else if ( !childrenAsPoints && points.Length == 0 )
                Debug.LogWarning(
                    HeaderStr +
                    "This spawner is set to use preset transforms as spheric spawn points " +
                    "but none where found"
                );
            else if ( points.Any( p => p.IsMissingOrNone() ) )
                Debug.LogWarning(
                    HeaderStr +
                    "This spawner is set to use preset transforms as spheric spawn points " +
                    "and one or more are missing or null"
                );
#endif

            if ( childrenAsPoints )
                points = transform.GetComponentsInChildren<Transform>();
        }

        _NextPosition = method switch
        {
            SpawnPosMethod.ExactPos             => rng => exactPos,
            SpawnPosMethod.InsideBounds         => rng => rng.InsideBounds( bounds ),
            SpawnPosMethod.InsideSpherePoints   => rng => {
                var pos = points[rng.Next(0, points.Length)].transform.position 
                        + rng.InsideSphere( sphereRadius );
                
                // Make sure it is on the north hemisphere
                // in case we actually want that.
                if ( northHemisphere )
                    pos.y = Mathf.Abs( pos.y ); 

                return pos;
            },

            _ => throw new NotImplementedException()
        };

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
        System.Random rng = new( Thread.CurrentThread.ManagedThreadId );

#if SPAWNER_DEBUG
        Debug.Log( HeaderStr + "Initiating SpawnLoop" );
#endif

        while ( true )
        {
#if SPAWNER_DEBUG
            Debug.Log( HeaderStr + "SpawnLoop called" );
#endif

            Vector3 position = _NextPosition( rng );

            if ( proceedSpawnCheck()
            && ( spawnChance == 1f || rng.NextFloat( 0f, 1f ) <= spawnChance ) )
            {
                beforeSpawn.Invoke();

                // Calculate probabilities
                float total = prefabsConfig.Sum( p => p.proportion );
            #if SPAWNER_DEBUG
                if ( total == 0f )
                    Debug.LogWarning( HeaderStr + "All prefabs have a proportion of 0, the first one will be spawned" );
            #endif
                float slice = rng.NextFloatInc() * total;
                float sum   = 0.0f;
                var p       = _pools[0];
                int i       = 0;
                for ( ; i < _pools.Length; i++ )
                {
                    if ( ( sum += prefabsConfig[i].proportion ) > slice )
                    {
                        p = _pools[i];
                        break;
                    }
                }

                // Spawn gameObject
                var bug = p.Get( position, Quaternion.identity, spawnedParent );
                afterSpawn.Invoke( bug.gameObject );
            }

            yield return new WaitForSeconds( waitLoopSecs );
        }
    }

    public void SetSpawnParent( Transform parent ) => this.spawnedParent = parent;

    public bool TrySetProportion( Pool p, float proportion )
    {
        int i = Array.IndexOf( _pools, p );
        if ( i == -1 )
            return false;

        prefabsConfig[i].proportion = Mathf.Clamp( proportion, 0f, 1f );
        return true;
    }

    public bool TrySetProportion( GameObject go, float proportion )
    {
        if ( !Pool.TryGetByInstance( go, out var p ) )
            return false;

        return TrySetProportion( p, proportion );
    }

    public void SetAllProportions( float proportion )
    {
        float p = Mathf.Clamp( proportion, 0f, 1f );
        foreach ( var conf in prefabsConfig )
            conf.proportion = p;
    }
}