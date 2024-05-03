using UnityEngine;

namespace BugCatcher.Utils
{
    /// <summary>
    /// Abstract Singleton class.
    /// Marks Instance as DontDestroyOnLoad by default.
    /// </summary>
    /// <typeparam name="T">Type that inherits MonoBehaviour</typeparam>
    public abstract class MonoSingle<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance { get; private set; } = null;

        void Awake()
        {
            if ( Instance is not null && Instance != this )
            {
                Destroy( gameObject );
                return;
            }

            Instance = this as T;
            DontDestroyOnLoad( gameObject );
            OnAwake();
        }

        /// <summary>
        /// OnAwake is called after Awake.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// OnDestroyCustom is called after assigning
        /// Instance to null OnDestroy.
        /// </summary>
        protected virtual void OnDestroyCustom() { }

        void OnDestroy()
        {
            Instance = null;
            OnDestroyCustom();
        }
    }

    /// <summary>
    /// Abstract Singleton class.
    /// Doesn't mark Instance as DontDestroyOnLoad, so by default,
    /// will be destroyed when loading a new scene.
    /// </summary>
    /// <typeparam name="T">Type that inherits MonoBehaviour</typeparam>
    public abstract class MonoShared<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance { get; private set; } = null;

        void Awake()
        {
            if ( Instance is not null && Instance != this )
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            OnAwake();
        }

        /// <summary>
        /// OnAwake is called after Awake.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// OnDestroyCustom is called after assigning
        /// Instance to null OnDestroy.
        /// </summary>
        protected virtual void OnDestroyCustom() { }

        void OnDestroy()
        {
            Instance = null;
            OnDestroyCustom();
        }
    }
}
