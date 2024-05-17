using System;

namespace BugCatcher.Extensions.Functional
{
    public static partial class Functional
    {
        public static TOut Map<TIn, TOut>( this TIn self, Func<TIn, TOut> f )
            => f( self );

        public static T Tee<T>( this T self, Action<T> a )
        {
            a( self );
            return self;
        }
    }
}