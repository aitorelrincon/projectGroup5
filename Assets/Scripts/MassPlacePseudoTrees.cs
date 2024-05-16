#define MPT_DEBUG

using BugCatcher.Extensions;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

using BugCatcher.Utils;

public class MassPlacePseudoTrees : MonoBehaviour
{
    #region Nested types
    public enum MassPlaceMode { Awake, Start, Manual }
    [Serializable] public class PrefabArgs 
        : ISerializationCallbackReceiver 
    {
        public GameObject   prefab;

        [Range(0, 1)]
        public float        proportion = 1f;

        bool _serialized = false;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() {
            if ( !_serialized ) {
                proportion  = 1f;
                _serialized = true;
            }
        }
    }
    #endregion

    #region Static properties
    public static readonly string Header = BC_Utils.Header<MassPlacePseudoTrees>();
    #endregion

    #region Properties
    public int spawnCount { 
        get => _spawnCount;
        set {
            WarnZero();
            _spawnCount = Mathf.Max( value, 0 );
        }
    }

    public GameObject[] instances { get; private set; }
    #endregion

    #region Mass Place Config
    [Header("Mass Place Config")]
    public MassPlaceMode            massPlaceMode   = MassPlaceMode.Awake;
    public Transform                spawnParent     = null;
    public Bounds                   spawnBounds;    // = new( Vector3.zero, new(30, 0, 30) );
    public Bounds                   avoidBounds;
    [SerializeField, Min(0)] int    _spawnCount     = 10;
    [SerializeField] PrefabArgs[]   _prefabs;
    #endregion

    #region Private members
    Terrain _terrain;
    #endregion

    #region Unity methods
    void Awake()
    {
        WarnZero();
        
        if ( massPlaceMode == MassPlaceMode.Awake )
            MassPlace();
    }

    void Start()
    {
        if ( massPlaceMode == MassPlaceMode.Start )
            MassPlace();
    }
    #endregion

    #region Public methods
    public void MassPlace()
    {
        if ( spawnCount == 0 || _prefabs.Length == 0 ) return;
        instances = new GameObject[ spawnCount ];

        // Calculate probabilities
        float       total = _prefabs.Sum( p => p.proportion );

        // Select and instantiate each GameObject
        for ( int i = 0; i < spawnCount; ++i )
        {
            UnityEngine.Debug.Log( "Hola?" );
            UnityEngine.Debug.Log( i );
            UnityEngine.Debug.Log( spawnCount );
            float slice = UnityEngine.Random.value * total;
            float sum   = 0.0f;
            GameObject prefab = _prefabs[ 0 ].prefab;
            foreach ( var p in _prefabs )
            {
                if ( ( sum += p.proportion ) > slice )
                {
                    prefab = p.prefab;
                    break;
                }
            }

            instances[i] = Instantiate( 
                prefab, 
                BC_Random.InsideBounds( spawnBounds ),
                Quaternion.Euler( prefab.transform.eulerAngles.WithY( UnityEngine.Random.Range( 0, 359 ) ) ),
                spawnParent
            );

            // If its too close to the player, snap out
            if ( avoidBounds.Contains( instances[i].transform.position ) )
            {
                var half = avoidBounds.extents;
                var pos  = instances[i].transform.position;

                // Select point closets to the outside of bounds
                // Changing both results in objects stacked on the Rect's vertices
                if ( pos.x < pos.z )
                    pos.x  = pos.x < half.x ? avoidBounds.min.x : avoidBounds.max.x;
                else
                    pos.z  = pos.z < half.z ? avoidBounds.min.z : avoidBounds.max.z;

                instances[i].transform.position = pos;
            }
        }
    }
#endregion

    #region Private methods
    [Conditional("MPT_DEBUG")]
    void WarnZero()
    {
        if ( spawnCount == 0 )
            UnityEngine.Debug.LogWarning( Header + "Count set to 0, trees won't be placed" );

        if ( _prefabs.Length == 0 )
            UnityEngine.Debug.LogWarning( Header + "No prefabs found" );
    }
    #endregion
}
