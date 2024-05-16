using UnityEngine;

public interface INetScaler<T>
    where T : MonoBehaviour
{
    void PreviousScale();
    void NextScale();
    void Scale( int index );
}
