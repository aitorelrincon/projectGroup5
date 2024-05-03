using System.Reflection;
using System.Linq;

/// <summary>
/// Static class containing const ints for Unity layers.
/// Simply copy & paste.
/// Meant to make switching on layers easier and all of that.
/// </summary>
public static class Layers
{
    public const int Default        = 0;
    public const int TransparentFX  = 1;
    public const int IgnoreRaycast  = 2;
    // public const int Layer3         = 3;
    public const int Water          = 4;
    public const int UI             = 5;



    /// <summary>
    /// Array access to the layers built with reflection.
    /// </summary>
    public static readonly int[] Array
        = typeof( Layers )
            .GetFields( BindingFlags.Public | BindingFlags.Static )
            .Where( (fi) => fi.IsLiteral
                         && !fi.IsInitOnly
                         && fi.FieldType == typeof(int) )
            .Select( (v) => (int)v.GetRawConstantValue() )
            .ToArray();
}