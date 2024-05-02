using System.Collections.Generic;
using UnityEngine;

namespace BugCatcher.Utils
{
    public class ObjectPool : MonoSingle<ObjectPool>
    {
        [Header("Object config")]
        [HideInInspector] public List<GameObject> objects = null;
        public GameObject original = null;
        public Transform defaultParent = null;
        [SerializeField, Min(0)] int _size;
        public int size
        {
            get => _size;
            set => _size = Mathf.Max( value, 0 );
        }

        [Header("Setup config")]
        public bool setupOnStart = true;

        public void ObjectsSetup()
        {
            objects = new( size );
            for ( int i = 0; i < _size; i++ )
            {
                var go = Instantiate(original, defaultParent);
                go.SetActive( false );
                objects.Add( go );
            }
        }

        void Start()
        {
            if ( setupOnStart ) ObjectsSetup();
        }

        public GameObject pooled { get => GetPooled(); }
        public GameObject GetPooled()
        {
            for ( int i = 0; i < _size; i++ )
            {
                if ( !objects[i].activeInHierarchy )
                {
                    return objects[i];
                }
            }

            return null;
        }

        public int activeCount { get => GetActiveCount(); }
        public int GetActiveCount()
        {
            int count = 0;

            for ( int i = 0; i < _size; i++ )
            {
                count += objects[i].activeInHierarchy ? 1 : 0;
            }

            return count;
        }

        public GameObject Activate( Vector3 position, Quaternion rotation, Transform parent )
        {
            var go = GetPooled();

            if ( go is not null )
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.transform.parent = parent;
                go.SetActive( true );
            }

            return go;
        }

        public GameObject Activate( Vector3 position, Quaternion rotation )
            => Activate( position, rotation, defaultParent );

        public GameObject Activate()
            => Activate( original.transform.position,
                         original.transform.rotation,
                         defaultParent );

        public void DeactivateAll()
        {
            for ( int i = 0; i < _size; ++i )
            {
                objects[i].SetActive( false );
            }
        }
    }
}