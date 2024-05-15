using UnityEngine;

public static class BC_Utils
{
    public static class Random
    {
        public static Vector3 InsideBounds( Bounds bounds )
            => new(
                UnityEngine.Random.Range( bounds.min.x, bounds.max.x ),
                UnityEngine.Random.Range( bounds.min.y, bounds.max.y ),
                UnityEngine.Random.Range( bounds.min.z, bounds.max.z )
            );
    }

    public static string Header<T>()
        => "[" + nameof( T ) + "] - ";
}
