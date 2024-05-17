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

        public static float NextFloatInc( this System.Random rng )
            => (float)rng.Next() / int.MaxValue;

        public static float NextFloatInc(this System.Random rng, float min, float max)
        {
            if (min > max)
                (min, max) = (max, min);

            return rng.NextFloatInc() * ( max - min ) + min;
        }

        /// <summary>
        /// Calculates a random point inside a sphere of radius 1.
        /// </summary>
        /// <param name="rng">system.Random</param>
        /// <returns>Random point inside a sphere of radius 1</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector3 InsideSphere( this System.Random rng )
        {
            float
                phi     = rng.NextFloatInc( 0f, 2f*Mathf.PI ),          // Azimuthal angle
                theta   = Mathf.Acos( rng.NextFloatInc( -1f, 1f ) ),    // Polar angle
                r       = Mathf.Pow( rng.NextFloatInc(), 1f / 3f );     // Radial distance

            float st = Mathf.Sin( theta );

            return new(
                    r * st * Mathf.Cos( phi ),
                    r * st * Mathf.Sin( phi ),
                    r * Mathf.Cos( theta )
                );
        }

        /// <summary>
        /// Calculates a random point inside a sphere of a given radius.
        /// </summary>
        /// <param name="rng">system.Random</param>
        /// <returns>Random point inside a sphere of a given radius</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector3 InsideSphere( this System.Random rng, float radius )
        {
            float
                phi     = rng.NextFloatInc( 0f, 2f * Mathf.PI ),                // Azimuthal angle
                theta   = Mathf.Acos( rng.NextFloatInc( -1f, 1f ) ),            // Polar angle
                r       = radius * Mathf.Pow( rng.NextFloatInc(), 1f / 3f );    // Radial distance

            float st = Mathf.Sin( theta );

            return new(
                    r * st * Mathf.Cos( phi ),
                    r * st * Mathf.Sin( phi ),
                    r * Mathf.Cos( theta )
                );
        }

        /// <summary>
        /// Returns a random position inside the specified Bounds.
        /// Uses System.Random.
        /// </summary>
        /// <param name="rng">System.Random instance</param>
        /// <param name="bounds">Bounds</param>
        /// <returns>Position inside Bounds</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InsideBounds( this System.Random rng, Bounds bounds )
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
        public static Vector3 InsideBounds( this Bounds bounds )
            => new(
                UnityEngine.Random.Range( bounds.min.x, bounds.max.x ),
                UnityEngine.Random.Range( bounds.min.y, bounds.max.y ),
                UnityEngine.Random.Range( bounds.min.z, bounds.max.z )
            );
    }
}