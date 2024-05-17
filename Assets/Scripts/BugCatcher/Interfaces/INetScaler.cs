using UnityEngine;

namespace BugCatcher.Interfaces
{
    public interface INetScaler<T>
        where T : MonoBehaviour
    {
        void PreviousScale();
        void NextScale();
        void SetScale( int index );
    }
}