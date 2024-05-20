using BugCatcher.Extensions.Functional;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace BugCatcher.Utils.ObjectPooling
{
    [DisallowMultipleComponent]
    public class PoolResource
    : MonoBehaviour
    {
        public Pool Pool       { get; private set; }
        public bool IsTemplate {
            get =>
                ( Pool is null )
                .Tee( ( b ) => Debug.Log( "[PoolResource] - Pool is null?: " + b ) )
                .Map( _ => Pool )    
                // .Map( _ => { Pool.TryGetByInstance( gameObject, out var pool ); return pool; } )
                .IsTemplate( gameObject ); 
        }

        public bool IsInit     { get; private set; }  = false;
        public bool ReturnOnDisable                   = true;

        public UnityEvent OnGetEvent;
        public UnityEvent OnReturnEvent;

        void Awake() => IsInit = true;
        public void OnGet()
        {
            if ( !IsInit ) return;
            OnGetEvent.Invoke();
        }

        public void OnReturn() => OnReturnEvent.Invoke();

        void OnDisable()
        {
            bool validPool  = Pool.IsValid( Pool ),
                 isInstance = !IsTemplate;
            if ( ReturnOnDisable 
            &&   validPool 
            &&   isInstance ) 
                Pool.Return( gameObject );

            Debug.LogFormat( 
                "[PoolResource] - OnDisable called for {0} ({1}, {2}, {3})", 
                gameObject.name,
                ReturnOnDisable,
                validPool,
                isInstance
            );
        }

        void OnDestroy()
        {
            if ( !Pool.IsValid( Pool ) ) return; // This means it has already been removed from its Pool

            if ( IsTemplate )
            {
                Debug.Log( $"[PoolResource] - Destroying {gameObject.name} prefab clears the Pool!" );
                Pool.Clear();
            }
            else
                Pool.Remove( gameObject );
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming style
        void ___SetPool( Pool pool )
        {
            Debug.Log( $"[PoolResource] - ___SetPool called for {gameObject.name} - pool: { pool is not null }" );
            Pool = pool;
        }

        [Obsolete]
        void ___MakeTemplate(  )
        {
            Debug.LogWarning( $"[PoolResource] - ___MakeTemplate IS OBSOLETE, REMOVE FROM CALLER" );
        }
#pragma warning restore IDE1006 // Naming style
#pragma warning restore IDE0051 // Remove unused private members
    }
}
