using System.Runtime.CompilerServices;
using UnityEngine;

namespace BugCatcher.Utils
{
    public static class BC_Utils
    {
        public static string Header<T>()
            => "[" + nameof( T ) + "] - ";
    }
}
