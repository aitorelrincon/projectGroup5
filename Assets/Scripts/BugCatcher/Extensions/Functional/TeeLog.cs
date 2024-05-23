using UnityEngine;

namespace BugCatcher.Extensions.Functional
{
    public static partial class Functional
    {
        public static T TeeLog<T>( this T self ) 
            => self.Tee( @in => Debug.Log( @in ) );

        public static T TeeLog<T>( this T self, Object ctx ) 
            => self.Tee( @in => Debug.Log( @in, ctx ) );
    }
}
