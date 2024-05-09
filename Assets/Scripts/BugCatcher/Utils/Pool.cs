// #define A_NIGHTMARE_ON_OOP_STREET

using BugCatcher.Extensions;
using BugCatcher.Interfaces;
using Oculus.Interaction.DebugTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

using System.Reflection;
using Unity.VisualScripting;
using Meta.WitAi.Events;

namespace BugCatcher.Utils.ObjectPooling
{
    public class Pool
    {
        const int DEFAULT_POOL_CAP      = 64;
        const int DEFAULT_PREFDICT_CAP  = DEFAULT_POOL_CAP;
        const int DEFAULT_INSTDICT_CAP  = 256;

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

        HashStack<PoolResource> _available;
        List<GameObject>        _instances;
        (GameObject gameObject, Quaternion rotation, Vector3 localScale)          
            _prefab;
        
        PoolResource            _template;

        public Pool( GameObject original ) 
        {
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
            if ( _templateSetup is null )
            {
                var go = new GameObject( "PoolsTemplateSetup" );
                Object.DontDestroyOnLoad( go );
                _templateSetup = go.transform;
            }

            _template.transform.parent = _templateSetup;
            if ( _templateSetup.childCount > 1 )
                Debug.LogError( "[Pools] - PoolsTemplateSetup must only have one child at a time" );

            _templateSetup.BroadcastMessage( "___SetPoolAndTemplate", this );
#endif

            _template.transform.parent = _prefabsParent;
            _available = new( DEFAULT_POOL_CAP );
            _instances = new( DEFAULT_POOL_CAP );
            _prefabsDict.Add( _prefab.gameObject, this );
        }

        ~Pool()
        {
            if ( _prefab.gameObject != null
            &&   _prefabsDict.ContainsKey( _prefab.gameObject ) )
                Clear( true );
        }

        public static Pool GetByPrefab( GameObject prefab, bool create = true )
        {
            if ( !_prefabsDict.TryGetValue( prefab, out var pool )
            &&   create )
                pool = new( prefab );

            return pool;
        }

        public static bool TryGetByInstance( GameObject instance, out Pool pool )
            => _instancesDict.TryGetValue( instance, out pool );

        public static bool TryReturnInstance( GameObject instance )
        {
            if ( !TryGetByInstance( instance, out Pool pool ) )
                return false;

            var r = instance.GetComponent<PoolResource>();
            pool.ResetResource( r );
            pool._available.Push( r );
            return true;
        }

        public static bool TryRemoveInstance( GameObject instance )
        {
            if ( !TryGetByInstance( instance, out Pool pool ) )
                return false;

            pool.Remove( instance );
            return true;
        }
        
        public static Pool Fill( GameObject prefab, int count )
        {
            var pool = GetByPrefab( prefab, true );
            pool.Fill( count );
            return pool;
        }

        public static void ClearAll()
        {
            foreach( var pool in _instancesDict.Values )
                pool.Clear();

            Debug.Log( $"[Pool] - All pools cleared" );
        }

        public void Fill( int count )
        {
            for ( int i = 0; i < count; i++ )
                _available.Push( CreateInstance( false ) );
        }

        public GameObject Get()
        {
            PoolResource instance;
            if ( _available.Count == 0 )
            {
                instance = CreateInstance();
                goto Finish;
            }

            // Find an available instance
            // Objects can only be removed from Stacks by popping.
            // Thus, we must make sure that each and every object we're
            // popping hasn't been destroyed yet.
            while (( instance = _available.Pop() ) != null && _available.Count > 0) { }

            // No available instances, create a new one
            if ( instance != null )
                instance = CreateInstance();

        Finish:
            instance.OnGet();
            instance.gameObject.SetActive( true );
            return instance.gameObject;
        }
        public GameObject Get( Transform parent )
        {
            var instance = Get();
            instance.transform.parent = parent;
            return instance;
        }

        public GameObject Get( Transform parent, bool worldPositionStays )
        {
            var instance = Get();
            instance.transform.SetParent( parent, worldPositionStays );
            return instance;
        }

        public GameObject Get( Vector3 position, Quaternion rotation )
        {
            var instance = Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }

        public GameObject Get( Vector3 position, Quaternion rotation, Transform parent )
        {
            var instance = Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.parent   = parent;
            return instance;
        }

        public T Get<T>() where T : Component 
            => Get().GetComponent<T>();

        public T Get<T>( Transform parent ) where T : Component
            => Get( parent ).GetComponent<T>();

        public T Get<T>( Transform parent, bool worldPositionStays )
            where T : Component
            => Get( parent, worldPositionStays )
                .GetComponent<T>();

        public T Get<T>( Vector3 position, Quaternion rotation )
            where T : Component
            => Get( position, rotation )
                .GetComponent<T>();

        public T Get<T>( Vector3 position, Quaternion rotation, Transform parent )
            where T : Component
            => Get( position, rotation, parent )
                .GetComponent<T>();

        public void Return( GameObject instance )
        {
            if ( !_instancesDict.ContainsKey( instance ) )
                return;

            var r = instance.GetComponent<PoolResource>();
            ResetResource( r );
            _available.Push( r );
        }

        public void Remove( GameObject instance )
        {
            if ( !_instancesDict.Remove( instance ) )
                return;

            ResetResource( instance.GetComponent<PoolResource>() );
            Object.Destroy( instance );
            _instancesDict.Remove( instance );
        }

        public void Clear( bool destroyInstances = true )
        {
            for ( int i = 0; i < _instances.Count; ++i )
            {
                var inst = _instances[i];
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

        PoolResource CreateInstance( bool active = true )
        {
            var instance = Object.Instantiate( _template );

            instance.gameObject.SetActive( active );
            _instancesDict.Add( instance.gameObject, this );
            _instances.Add( instance.gameObject );

            return instance;
        }

        void ResetResource( PoolResource resource )
        {
            resource.OnReturn();
            resource.gameObject.SetActive( false );

            resource.transform.parent     = null;
            resource.transform.rotation   = _prefab.rotation;
            resource.transform.localScale = _prefab.localScale;
        }
    }
}