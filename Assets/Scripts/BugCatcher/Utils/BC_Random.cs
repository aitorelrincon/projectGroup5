using BugCatcher.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BugCatcher.Utils
{

    public static class BC_Random
    {
        /// <summary>
        /// Extension method for System.Random.
        /// Returns a random float between 0f and 1f.
        /// </summary>
        /// <param name="rng">System.Random instance</param>
        /// <returns>Random float between 0f and 1f</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float NextFloat( this System.Random rng )
            => (float)rng.NextDouble();

        /// <summary>
        /// Extension method for System.Random.
        /// Returns a random float between min and max (inclusive).
        /// When min > max, they're swapped.
        /// </summary>
        /// <param name="rng">System.Random instance</param>
        /// <param name="min">Min float value</param>
        /// <param name="max">Max float value</param>
        /// <returns>Random float between min and max.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float NextFloat( this System.Random rng, float min, float max )
        {
            if ( min > max )
                (min, max) = (max, min);

            return (float)rng.NextDouble() * ( max - min ) + min;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector3 NextUnitSphere( this System.Random rng )
        {
            float unit = rng.NextFloat(0, 2);
            return BC_Vecs.Fill3( unit );
        }

        /// <summary>
        /// Returns a random position inside the specified Bounds.
        /// Uses System.Random.
        /// </summary>
        /// <param name="rng">System.Random instance</param>
        /// <param name="bounds">Bounds</param>
        /// <returns>Position inside Bounds</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsideBounds( System.Random rng, Bounds bounds )
            => new(
                rng.NextFloat( bounds.min.x, bounds.max.x ),
                rng.NextFloat( bounds.min.y, bounds.max.y ),
                rng.NextFloat( bounds.min.z, bounds.max.z )
            );

        /// <summary>
        /// Returns a random position inside the specified Bounds.
        /// Uses UnityEngine.Random.
        /// </summary>
        /// <param name="bounds">Bounds</param>
        /// <returns>Position inside Bounds</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsideBounds( Bounds bounds )
            => new(
                UnityEngine.Random.Range( bounds.min.x, bounds.max.x ),
                UnityEngine.Random.Range( bounds.min.y, bounds.max.y ),
                UnityEngine.Random.Range( bounds.min.z, bounds.max.z )
            );
    }
}