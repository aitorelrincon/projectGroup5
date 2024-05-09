using System.Collections;
using System.Collections.Generic;
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
            if ( ReturnOnDisable ) Pool.Return( gameObject );
        }

        void OnDestroy()
        {
            if ( IsTemplate )
                Debug.Log( $"[PoolResource] - Destroying {gameObject.name} prefab" );

            Pool.Remove( gameObject );
        }
    
        void ___SetPoolAndTemplate( Pool pool )
        {
            Pool       = pool;
            IsTemplate = true;
        }
    }
}
