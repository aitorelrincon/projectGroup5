#define BC_VECS_EXTENSION

using System.Runtime.CompilerServices;
using UnityEngine;

using UV2 = UnityEngine.Vector2;
using UV2I = UnityEngine.Vector2Int;
using UV3 = UnityEngine.Vector3;
using UV3I = UnityEngine.Vector3Int;
using UV4 = UnityEngine.Vector4;

using SNV2 = System.Numerics.Vector2;
using SNV3 = System.Numerics.Vector3;
using SNV4 = System.Numerics.Vector4;
using System;

namespace BugCatcher.Extensions
{
    public static class BC_Vecs
    {
        public struct Bool3
        {
            public bool x, y, z;

            public Bool3( bool b )
            {
                x = b;
                y = b;
                z = b;
            }

            public Bool3( bool x, bool y, bool z )
            {
                this.x = x; 
                this.y = y; 
                this.z = z;
            }

            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            public bool All() => x && y && z;
            
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            public bool Any() => x || y || z;

            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            public static implicit operator bool( Bool3 b3 ) => b3.All();
        }

#if BC_VECS_EXTENSION
        #region UnityEngine's Vectors extensions
        // Set[Axis]
        // X
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetX( this ref UV2 v, float newX ) => v.x = newX;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetX( this ref UV2I v, int newX ) => v.x = newX;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetX( this ref UV3 v, float newX ) => v.x = newX;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetX( this ref UV3I v, int newX ) => v.x = newX;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetX( this ref UV4 v, float newX ) => v.x = newX;

        // Y
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetY( this ref UV2 v, float newY ) => v.y = newY;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetY( this ref UV2I v, int newY ) => v.y = newY;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetY( this ref UV3 v, float newY ) => v.y = newY;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetY( this ref UV3I v, int newY ) => v.y = newY;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetY( this ref UV4 v, float newY ) => v.y = newY;

        // Z
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetZ( this ref UV3 v, float newZ ) => v.z = newZ;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetZ( this ref UV3I v, int newZ ) => v.z = newZ;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetZ( this ref UV4 v, float newZ ) => v.z = newZ;

        // W
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void SetW( this ref UV4 v, float newW ) => v.w = newW;

        // With[Axis]
        // X
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 WithX( this UV2 v, float newX ) => new( newX, v.y );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I WithX( this UV2I v, int newX ) => new( newX, v.y );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithX( this UV3 v, float newX ) => new( newX, v.y, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithX( this UV3I v, int newX ) => new( newX, v.y, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithX( this UV4 v, float newX ) => new( newX, v.y, v.z, v.w );

        // Y
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 WithY( this UV2 v, float newY ) => new( v.x, newY );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I WithY( this UV2I v, int newY ) => new( v.x, newY );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithY( this UV3 v, float newY ) => new( v.x, newY, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithY( this UV3I v, int newY ) => new( v.x, newY, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithY( this UV4 v, float newY ) => new( v.x, newY, v.z, v.w );

        // Z
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithZ( this UV3 v, float newZ ) => new( v.x, v.y, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithZ( this UV3I v, int newZ ) => new( v.x, v.y, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithZ( this UV4 v, float newZ ) => new( v.x, v.y, newZ, v.w );

        // W
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithW( this UV4 v, float newW ) => new( v.x, v.y, v.z, newW );

        // With[Axes]
        // XY
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithXY( this UV3 v, float newX, float newY ) => new( newX, newY, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithXY( this UV3I v, int newX, int newY ) => new( newX, newY, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithXY( this UV4 v, float newX, float newY ) => new( newX, newY, v.z, v.w );

        // XZ
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithXZ( this UV3 v, float newX, float newZ ) => new( newX, v.y, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithXZ( this UV3I v, int newX, int newZ ) => new( newX, v.y, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithXZ( this UV4 v, float newX, float newZ ) => new( newX, v.y, newZ, v.w );

        // YZ
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 WithYZ( this UV3 v, float newY, float newZ ) => new( v.x, newY, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I WithYZ( this UV3I v, int newY, int newZ ) => new( v.x, newY, newZ );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithYZ( this UV4 v, float newY, float newZ ) => new( v.x, newY, newZ, v.w );

        // XW
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithXW( this UV4 v, float newX, float newW ) => new( newX, v.y, v.z, newW );

        // YW
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithYW( this UV4 v, float newY, float newW ) => new( v.x, newY, v.z, newW );

        // ZW
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithZW( this UV4 v, float newZ, float newW ) => new( v.x, v.y, newZ, newW );

        // XYZ
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithXYZ( this UV4 v, float newX, float newY, float newZ ) => new( newX, newY, newZ, v.w );

        // XZW
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithXZW( this UV4 v, float newX, float newZ, float newW ) => new( newX, v.y, newZ, newW );

        // YZW
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 WithYZW( this UV4 v, float newY, float newZ, float newW ) => new( v.x, newY, newZ, newW );


        // System.Numerics.Vector & UnityEngine.Vector
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static SNV2 ToNumerics( this UV2 v ) => new SNV2( v.x, v.y );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static SNV3 ToNumerics( this UV3 v ) => new SNV3( v.x, v.y, v.z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static SNV4 ToNumerics( this UV4 v ) => new SNV4( v.x, v.y, v.z, v.w );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 ToUnity( this SNV2 v ) => new UV2( v.X, v.Y );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 ToUnity( this SNV3 v ) => new UV3( v.X, v.Y, v.Z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 ToUnity( this SNV4 v ) => new UV4( v.X, v.Y, v.Z, v.W );

        // Map (Vector map)
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 MapV( this UV2 v, Func<float, float> f ) => new( f( v.x ), f( v.y ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I MapV( this UV2I v, Func<int, int> f ) => new( f( v.x ), f( v.y ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 MapV( this UV3 v, Func<float, float> f ) => new( f( v.x ), f( v.y ), f( v.z ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I MapV( this UV3I v, Func<int, int> f ) => new( f( v.x ), f( v.y ), f( v.z ) );
        
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 MapV( this UV4 v, Func<float, float> f ) => new( f( v.x ), f( v.y ), f( v.z ), f( v.w ) );

        // MapI (Vector map with index)
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 MapI( this UV2 v, Func<float, int, float> f ) => new( f( v.x, 0 ), f( v.y, 1 ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I MapI( this UV2I v, Func<int, int, int> f )   => new( f( v.x, 0 ), f( v.y, 1 ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 MapI( this UV3 v, Func<float, int, float> f ) => new( f( v.x, 0 ), f( v.y, 1 ), f( v.z, 2 ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I MapI( this UV3I v, Func<int, int, int> f )   => new( f( v.x, 0 ), f( v.y, 1 ), f( v.z, 2 ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 MapI( this UV4 v, Func<float, int, float> f ) => new( f( v.x, 0 ), f( v.y, 1 ), f( v.z, 2 ), f( v.w, 3 ) );

        // MapI (Vector map with index)
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 ZipMapV( this UV2 v, UV2 u, Func<float, float, float> f ) => new( f( v.x, u.x ), f( v.y, u.y ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I ZipMapV( this UV2I v, UV2I u, Func<int, int, int> f )    => new( f( v.x, u.x ), f( v.y, u.y ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 ZipMapV( this UV3 v, UV3 u, Func<float, float, float> f ) => new( f( v.x, u.x ), f( v.y, u.y ), f( v.z, u.z ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I ZipMapV( this UV3I v, UV3I u, Func<int, int, int> f )    => new( f( v.x, u.x ), f( v.y, u.y ), f( v.z, u.z ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV4 ZipMapV( this UV4 v, UV4 u, Func<float, float, float> f ) => new( f( v.x, 0 ), f( v.y, u.y ), f( v.z, u.z ), f( v.w, u.w ) );
        #endregion
#else
        #region Extensions as static methods
        // Set[Axis]
        // X
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX( ref UV2 v, float newX) => v.x = newX;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX( ref UV2I v, int newX)  => v.x = newX;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX( ref UV3 v, float newX) => v.x = newX;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX( ref UV3I v, int newX)  => v.x = newX;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetX( ref UV4 v, float newX) => v.x = newX;

        // Y
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY( ref UV2 v, float newY) => v.y = newY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY( ref UV2I v, int newY)  => v.y = newY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY( ref UV3 v, float newY) => v.y = newY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY( ref UV3I v, int newY)  => v.y = newY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetY( ref UV4 v, float newY) => v.y = newY;

        // Z
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZ( ref UV3 v, float newZ) => v.z = newZ;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZ( ref UV3I v, int newZ)  => v.z = newZ;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZ( ref UV4 v, float newZ) => v.z = newZ;

        // W
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetW( ref UV4 v, float newW) => v.w = newW;

        // With[Axis]
        // X
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV2 WithX(UV2 v, float newX) => new(newX, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV2I WithX(UV2I v, int newX) => new(newX, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithX(UV3 v, float newX) => new(newX, v.y, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithX(UV3I v, int newX) => new(newX, v.y, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithX(UV4 v, float newX) => new(newX, v.y, v.z, v.w);

        // Y
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV2 WithY(UV2 v, float newY) => new(v.x, newY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV2I WithY(UV2I v, int newY) => new(v.x, newY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithY(UV3 v, float newY) => new(v.x, newY, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithY(UV3I v, int newY) => new(v.x, newY, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithY(UV4 v, float newY) => new(v.x, newY, v.z, v.w);

        // Z
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithZ(UV3 v, float newZ) => new(v.x, v.y, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithZ(UV3I v, int newZ) => new(v.x, v.y, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithZ(UV4 v, float newZ) => new(v.x, v.y, newZ, v.w);

        // W
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithW(UV4 v, float newW) => new(v.x, v.y, v.z, newW);

        // With[Axes]
        // XY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithXY(UV3 v, float newX, float newY) => new(newX, newY, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithXY(UV3I v, int newX, int newY) => new(newX, newY, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithXY(UV4 v, float newX, float newY) => new(newX, newY, v.z, v.w);

        // XZ
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithXZ(UV3 v, float newX, float newZ) => new(newX, v.y, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithXZ(UV3I v, int newX, int newZ) => new(newX, v.y, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithXZ(UV4 v, float newX, float newZ) => new(newX, v.y, newZ, v.w);

        // YZ
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 WithYZ(UV3 v, float newY, float newZ) => new(v.x, newY, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3I WithYZ(UV3I v, int newY, int newZ) => new(v.x, newY, newZ);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithYZ(UV4 v, float newY, float newZ) => new(v.x, newY, newZ, v.w);

        // XW
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithXW(UV4 v, float newX, float newW) => new(newX, v.y, v.z, newW);

        // YW
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithYW(UV4 v, float newY, float newW) => new(v.x, newY, v.z, newW);

        // ZW
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithZW(UV4 v, float newZ, float newW) => new(v.x, v.y, newZ, newW);

        // XYZ
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithXYZ(UV4 v, float newX, float newY, float newZ) => new(newX, newY, newZ, v.w);

        // XZW
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithXZW(UV4 v, float newX, float newZ, float newW) => new(newX, v.y, newZ, newW);

        // YZW
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 WithYZW(UV4 v, float newY, float newZ, float newW) => new(v.x, newY, newZ, newW);


        // System.Numerics.Vector & UnityEngine.Vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SNV2 ToNumerics(UV2 v) => new SNV2(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SNV3 ToNumerics(UV3 v) => new SNV3(v.x, v.y, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SNV4 ToNumerics(UV4 v) => new SNV4(v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV2 ToUnity(SNV2 v) => new UV2(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV3 ToUnity(SNV3 v) => new UV3(v.X, v.Y, v.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UV4 ToUnity(SNV4 v) => new UV4(v.X, v.Y, v.Z, v.W);
        #endregion
#endif // GRPU_VECS_EXTENSION

        #region Static methods
        // Fill
        /// <summary>
        /// Constructs a Vector2 with n as all of its components.
        /// </summary>
        /// <param name="n">Number to fill the Vector2 with.</param>
        /// <returns>Vector2 with n as all of its components.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2 Fill2( float n ) => new UV2( n, n );

        /// <summary>
        /// Constructs a Vector2Int with n as all of its components.
        /// </summary>
        /// <param name="n">Number to fill the Vector2Int with.</param>
        /// <returns>Vector2Int with n as all of its components.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV2I Fill2Int( int n ) => new UV2I( n, n );

        /// <summary>
        /// Constructs a Vector3 with n as all of its components.
        /// </summary>
        /// <param name="n">Number to fill the Vector3 with.</param>
        /// <returns>Vector3 with n as all of its components.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 Fill3( float n ) => new UV3( n, n, n );

        /// <summary>
        /// Constructs a Vector3Int with n as all of its components.
        /// </summary>
        /// <param name="n">Number to fill the Vector3Int with.</param>
        /// <returns>Vector3Int with n as all of its components.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3I Fill3Int( int n ) => new UV3I( n, n, n );

        /// <summary>
        /// Constructs a Vector4 with n as all of its components.
        /// </summary>
        /// <param name="n">Number to fill the Vector4
        /// with.</param>
        /// <returns>Vector4
        /// with n as all of its components.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static UV3 Fill4( float n ) => new UV4( n, n, n, n );
        #endregion
    }
}