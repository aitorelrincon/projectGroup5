using UnityEngine;
using UnityEngine.Events;

namespace BugCatcher.Utils.ObjectPooling
{
    [DisallowMultipleComponent]
    public class PoolResource
    : MonoBehaviour
    {
        public Pool Pool       { get; private set; }
        public bool IsTemplate { get; private set; }  = false;
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
            if ( ReturnOnDisable && Pool is not null ) 
                Pool.Return( gameObject );
        }

        void OnDestroy()
        {
            if ( Pool is null ) return; // This means it has already been removed from its Pool

            if ( IsTemplate )
                Debug.Log( $"[PoolResource] - Destroying {gameObject.name} prefab" );

            Pool.Remove( gameObject );
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming style
        void ___SetPool( Pool pool )
        {
            Debug.Log( $"[PoolResource] - ___SetPool called for {gameObject.name} - pool: { pool is not null }" );
            Pool = pool;
        }

        void ___MakeTemplate(  )
        {
            Debug.Log( $"[PoolResource] - ___MakeTemplate called for {gameObject.name}" );
            IsTemplate = true;
        }
#pragma warning restore IDE1006 // Naming style
#pragma warning restore IDE0051 // Remove unused private members
    }
}
