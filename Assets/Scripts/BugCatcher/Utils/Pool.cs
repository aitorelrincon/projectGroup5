// #define A_NIGHTMARE_ON_OOP_STREET

using BugCatcher.Extensions;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace BugCatcher.Utils.ObjectPooling
{
    public class Pool
    {
        #region Constants
        const int DEFAULT_POOL_CAP      = 64;
        const int DEFAULT_PREFDICT_CAP  = DEFAULT_POOL_CAP;
        const int DEFAULT_INSTDICT_CAP  = 256;
        public static readonly Pool Sentinel   = new();
        #endregion

        #region Private static variables
#if A_NIGHTMARE_ON_OOP_STREET
        static MethodInfo _setTemplateAndPool = 
            typeof(PoolResource )
                .GetMethod( "___SetPoolAndTemplate", BindingFlags.Instance | BindingFlags.NonPublic );
#else
        static Transform                    _templateSetup = null;
#endif

        static Transform                    _prefabsParent = null;
        static Dictionary<GameObject, Pool>   _prefabsDict = new( DEFAULT_PREFDICT_CAP );
        static Dictionary<GameObject, Pool> _instancesDict = new( DEFAULT_INSTDICT_CAP );
        #endregion

        #region Instance variables
        HashStack<PoolResource> _available;
        List<GameObject>        _instances;
        (GameObject gameObject, Quaternion rotation, Vector3 localScale)          
            _prefab;
        
        PoolResource            _template;
        #endregion

        #region Properties
        public int  PooledCount { get => _available.Count; }
        public int  TotalCount  { get => _instances.Count; }
        #endregion

        #region Constructors & destructor
        /// <summary>
        /// Sentinel Pool constructor
        /// </summary>
        private Pool() { }

        /// <summary>
        /// Constructs a new object Pool for the passed object.
        /// Automatically adds a PoolResource component if it
        /// doesn't have one already.
        /// </summary>
        /// <param name="original">Original GameObject</param>
        /// <param name="capacity">Pool capacity</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Capacity must be > 0</exception>
        /// <exception cref="System.ArgumentException">Pool already exists for that object</exception>
        private Pool( GameObject original, int capacity = DEFAULT_POOL_CAP ) 
        {
            if ( capacity < 0 )
                throw new System.ArgumentOutOfRangeException( "Capacity must be > 0" );

        // If constructor is private, we don't have to worry about this
        #if false
            if ( _prefabsDict.ContainsKey( original ) )
                throw new System.ArgumentException( "Pool already exists for that object" );
        #endif

            if ( _prefabsParent is null )
            {
                var go = new GameObject( "PoolsPrefabsParent" );
                Object.DontDestroyOnLoad( go );
                _prefabsParent = go.transform;
            }

            _prefab   = ( original, original.transform.rotation, original.transform.localScale );
            _template = original.GetComponentOrElse(() => { 
                var r = Object.Instantiate( _prefab.gameObject )
                              .AddComponent<PoolResource>();
                
                return r;
            });
            
            // There are no friend classes, and working with multiple
            // assemblies gets REALLY messy.
            // Next .Invoke basically does this:
            // _template.Pool       = this;
            // _template.IsTemplate = true;
#if A_NIGHTMARE_ON_OOP_STREET
            _setTemplateAndPool.Invoke( _template, new[] { this } );
#else
            // ...Though a WAY less weird way of doing is just calling
            // BroadcastMessage with a temp parent.
            // Plus, some of the score for the project comes from doing
            // a justified use of both SendMessage & BroadcastMessage.
            // EDIT: Wait I'm stupid. We can just use SendMessage. Less expensive.
            if ( _templateSetup is null )
            {
                var go = new GameObject( "PoolsTemplateSetup" );
                Object.DontDestroyOnLoad( go );
                // _templateSetup = go.transform;
            }

            // _template.transform.parent = _templateSetup;
            // if ( _templateSetup.childCount > 1 )
            //  Debug.LogError( "[Pools] - PoolsTemplateSetup must only have one child at a time" );
            // _templateSetup.BroadcastMessage( "___SetPool", { this } );

            _template.SendMessage( "___SetPool", this );
            // _template.SendMessage( "___MakeTemplate" );
#endif

            _template.transform.parent = _prefabsParent;
            _available = new( capacity );
            _instances = new( capacity );
            _prefabsDict.Add( _prefab.gameObject, this );
        }

        /// <summary>
        /// Prefab entry on the _prefabDict shouldn't keep existing after destroying.
        /// </summary>
        ~Pool()
        {
            if ( _prefab.gameObject != null
            &&   _prefabsDict.ContainsKey( _prefab.gameObject ) )
                Clear( true );
        }
#endregion

        #region Static methods
        /// <summary>
        /// Retrieves the Pool associated with the passed prefab,
        /// or creates one by default if it doesn't exist.
        /// </summary>
        /// <param name="prefab">Original object</param>
        /// <param name="creationCapacity">Capacity when creating a new Pool</param>
        /// <returns></returns>
        public static Pool GetOrAdd( GameObject prefab, int creationCapacity = DEFAULT_POOL_CAP )
        {
            if ( TryGetByInstance( prefab, out var instPool ) )
                return instPool;

            if ( !TryGetByPrefab( prefab, out var prefPool ) )
                prefPool = new( prefab, creationCapacity );

            return prefPool;
        }

        /// <summary>
        /// Tries to retrive the Pool associated with the passed GameObject,
        /// only checking global pooled prefabs.
        /// </summary>
        /// <param name="instance">Prefab to find</param>
        /// <param name="pool">Pool to write</param>
        /// <returns>True if succesful</returns>
        public static bool TryGetByPrefab( GameObject prefab, out Pool pool )
            => _prefabsDict.TryGetValue( prefab, out pool );

        /// <summary>
        /// Tries to retrive the Pool associated with the passed GameObject,
        /// only checking global pooled instances.
        /// </summary>
        /// <param name="instance">Instance to find</param>
        /// <param name="pool">Pool to write</param>
        /// <returns>True if succesful</returns>
        public static bool TryGetByInstance( GameObject instance, out Pool pool )
            => _instancesDict.TryGetValue( instance, out pool );

        /// <summary>
        /// Tries to return an Instance to its respective Pool.
        /// </summary>
        /// <param name="instance">Instance to return</param>
        /// <returns>True if succesful</returns>
        public static bool TryReturnInstance( GameObject instance )
        {
            if ( !TryGetByInstance( instance, out Pool pool ) )
                return false;

            var r = instance.GetComponent<PoolResource>();
            pool.ResetResource( r );
            pool._available.Push( r );
            return true;
        }

        /// <summary>
        /// Tries to remove an Instance from its respective Pool.
        /// </summary>
        /// <param name="instance">Instance to remove</param>
        /// <returns>True if succesful</returns>
        public static bool TryRemoveInstance( GameObject instance )
        {
            if ( !TryGetByInstance( instance, out Pool pool ) )
                return false;

            pool.Remove( instance );
            return true;
        }
        
        /// <summary>
        /// Fills a Pool with 'count' instances of the 'prefab'.
        /// </summary>
        /// <param name="prefab">Original prefab</param>
        /// <param name="count">Instances to add</param>
        /// <returns></returns>
        public static Pool GetAndFill( GameObject prefab, int count )
        {
            var pool = GetOrAdd( prefab );
            pool.Fill( count );
            Debug.Log( count.ToString() + " - " + pool.TotalCount );
            return pool;
        }

        /// <summary>
        /// Clears all Pools.
        /// </summary>
        public static void ClearAll()
        {
            foreach( var pool in _instancesDict.Values )
                pool.Clear();

            Debug.Log( $"[Pool] - All pools cleared" );
        }

        public static bool IsValid( Pool pool ) => pool is not null && !ReferenceEquals( pool, Sentinel );
        #endregion

        #region Public instance methods
        /// <summary>
        /// Fills a Pool with 'count' instances of its prefab.
        /// </summary>
        /// <param name="count"></param>
        public void Fill( int count )
        {
            for ( int i = 0; i < count; i++ )
                _available.Push( CreateInstance( false ) );
        }

        /// <summary>
        /// Returns an Instance of the Pooled prefab, creating a new one
        /// when there are no more.
        /// </summary>
        /// <returns>Pooled Instance</returns>
        public GameObject Get()
        {
            PoolResource instance;

            // TODO: Preemptively creating more than one here might be a good idea
            if ( _available.Count == 0 )
            {
                instance = CreateInstance();
                goto Finish;
            }

            // Find an available instance
            // Objects can only be removed from Stacks by popping.
            // Thus, we must make sure that each and every object we're
            // popping hasn't been destroyed yet.
            while ( ( instance = _available.Pop() ) == null && _available.Count > 0 ) { }

            // No available instances, create a new one
            if ( instance == null )
                instance = CreateInstance();

        Finish:
            instance.OnGet();
            instance.gameObject.SetActive( true );
            return instance.gameObject;
        }

        /// <summary>
        /// <inheritdoc cref="Get()"/>
        /// Also, sets the parent of the object.
        /// </summary>
        /// <param name="parent">New parent</param>
        /// <returns>Pooled Instance</returns>
        public GameObject Get( Transform parent)
        {
            var instance = Get();
            instance.transform.parent = parent;
            return instance;
        }

        /// <summary>
        /// <inheritdoc cref="Get()"/>
        /// Also, sets the parent of the object, taking into account if it
        /// should keep the same world position.
        /// </summary>
        /// <param name="parent">New parent</param>
        /// <param name="worldPositionStays">Should keep same world position?</param>
        /// <returns>Pooled Instance</returns>
        public GameObject Get( Transform parent, bool worldPositionStays)
        {
            var instance = Get();
            instance.transform.SetParent( parent, worldPositionStays );
            return instance;
        }

        /// <summary>
        /// <inheritdoc cref="Get()"/>
        /// Also, sets both the position & rotation of the object.
        /// </summary>
        /// <param name="position">New position</param>
        /// <param name="rotation">New rotation</param>
        /// <returns>Pooled Instance</returns>
        public GameObject Get( Vector3 position, Quaternion rotation )
        {
            var instance = Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }

        /// <summary>
        /// <inheritdoc cref="Get()"/>
        /// Also, sets the position, rotation and parent of the object.
        /// </summary>
        /// <param name="position">New position</param>
        /// <param name="rotation">New rotation</param>
        /// <param name="parent">New parent</param>
        /// <returns>Pooled Instance</returns>
        public GameObject Get( Vector3 position, Quaternion rotation, Transform parent )
        {
            var instance = Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.parent   = parent;
            return instance;
        }

        /// <summary>
        /// <inheritdoc cref="Get()"/>
        /// After that, gets a Component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Pooled Instance's Component</returns>
        public T Get<T>() where T : Component 
            => Get().GetComponent<T>();

        /// <summary>
        /// <inheritdoc cref="Get( Transform )"/>
        /// After that, gets a Component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Pooled Instance's Component</returns>
        public T Get<T>( Transform parent ) where T : Component
            => Get( parent ).GetComponent<T>();

        /// <summary>
        /// <inheritdoc cref="Get( Transform, bool )"/>
        /// After that, gets a Component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Pooled Instance's Component</returns>
        public T Get<T>( Transform parent, bool worldPositionStays )
            where T : Component
            => Get( parent, worldPositionStays )
                .GetComponent<T>();

        /// <summary>
        /// <inheritdoc cref="Get( Transform, Quaternion )"/>
        /// After that, gets a Component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Pooled Instance's Component</returns>
        public T Get<T>( Vector3 position, Quaternion rotation )
            where T : Component
            => Get( position, rotation )
                .GetComponent<T>();

        /// <summary>
        /// <inheritdoc cref="Get( Transform, Quaternion, Transform )"/>
        /// After that, gets a Component of the specified type.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Pooled Instance's Component</returns>
        public T Get<T>( Vector3 position, Quaternion rotation, Transform parent )
            where T : Component
            => Get( position, rotation, parent )
                .GetComponent<T>();

        /// <summary>
        /// Returns an Instance to its Pool.
        /// </summary>
        /// <param name="instance">Instance to return</param>
        public void Return( GameObject instance )
        {
            if ( !_instances.Contains( instance ) )
                return;

            var r = instance.GetComponent<PoolResource>();
            ResetResource( r );
            _available.Push( r );
        }

        /// <summary>
        /// Remove an Instance from its Pool.
        /// </summary>
        /// <param name="instance">Instance to remove</param>
        /// <param name="destroyInstance">Should destroy instance?</param>
        public void Remove( GameObject instance, bool destroyInstance = true )
        {
            if ( !_instances.Remove( instance ) )
                return;

            var p = instance.GetComponent<PoolResource>();
            p.SendMessage( "___SetPool", Sentinel ); // Avoid calling Remove recursively OnDestroy
            ResetResource( p );
            Object.Destroy( p );
            
            if ( destroyInstance || !instance.activeInHierarchy )
                Object.Destroy( instance );

            _instancesDict.Remove( instance );
        }

        /// <summary>
        /// Clears the Pool.
        /// </summary>
        /// <param name="destroyInstances">Should destroy all instances?</param>
        public void Clear( bool destroyInstances = true )
        {
            for ( int i = 0; i < _instances.Count; ++i )
            {
                var inst = _instances[i];

                // Already destroyed; ignore
                if ( inst == null )
                    continue;

                _instancesDict.Remove( inst );

                if ( destroyInstances || !inst.activeInHierarchy )
                    Object.Destroy( inst );
            }

            _prefabsDict.Remove( _prefab.gameObject );

            _prefab     = ( null, default, default );
            _template   = null;
            _available  = null;
            _instances  = null;
        
            Debug.Log( $"[Pool] - Pool for {_prefab.gameObject.name} has been cleared" );
        }

        public bool IsTemplate( GameObject gameObject ) => gameObject == _template.gameObject;
        public bool IsTemplate( PoolResource resource ) => resource   == _template;
        #endregion

        #region Private instance methods
        /// <summary>
        /// Creates a new Instance of the Pooled prefab.
        /// </summary>
        /// <param name="active">SetActive</param>
        /// <returns>New Instance</returns>
        PoolResource CreateInstance( bool active = true )
        {
            var instance = Object.Instantiate( _template );

            instance.gameObject.SetActive( active );
            _instancesDict.Add( instance.gameObject, this );
            _instances.Add( instance.gameObject );

            return instance;
        }

        /// <summary>
        /// Resets PoolResource
        /// </summary>
        /// <param name="resource">PoolResource</param>
        void ResetResource( PoolResource resource )
        {
            resource.OnReturn();
            resource.gameObject.SetActive( false );

            // TODO: Maybe we could have a default parent for Pooled objects...
            resource.transform.parent     = null;
            resource.transform.rotation   = _prefab.rotation;
            resource.transform.localScale = _prefab.localScale;
        }
        #endregion
    }
}