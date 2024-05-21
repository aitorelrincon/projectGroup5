using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BugCatcher.Utils.ObjectPooling
{
    public class PoolParenting : MonoBehaviour
    {
        public const int DEFAULT_CAP = 256;
        Queue<(GameObject go, Transform t)> queue = new( DEFAULT_CAP );

        void Start()
        {
            StartCoroutine( ReparentNextFrame() );
        }

        public void Enqueue( GameObject go, Transform p )
        {
            if ( !queue.Contains( (go, p) ) )
                queue.Enqueue( (go, p) );
        }

        IEnumerator ReparentNextFrame()
        {
            while ( true )
            {
                yield return new WaitForEndOfFrame();
                while ( queue.TryDequeue( out var current ) )
                    current.go.transform.SetParent( current.t );
            }
        }
    }

}