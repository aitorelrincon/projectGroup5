using UnityEngine;
using System.Runtime.CompilerServices;

namespace BugCatcher.Extensions.Functional
{
    public static partial class Functional
    {
#if DEBUG
        public static T TeeLog<T>( this T self )
            => self.Tee( @in => Debug.Log( @in ) );

        public static T TeeLog<T>( this T self, Object ctx ) 
            => self.Tee( @in => Debug.Log( @in, ctx ) );
#else
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TeeLog<T>( this T self ) => self;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TeeLog<T>( this T self, Object ctx ) => self;
#endif
    }
}
