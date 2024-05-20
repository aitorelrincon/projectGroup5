using System.Text;
using System.Reflection;
using UnityEngine;

using BugCatcher.Extensions;

using MovementKind = MovementParams.MovementKind;
using static MovementParams;
using System;

[DefaultExecutionOrder(-9998)]
public class BugBehaviourParams : MonoBehaviour
{
#if !UNITY_EDITOR
    void Awake() => Destroy( this );
#else
    #region Stat array names
    const string s_Speed            = "_Speed";
    const string s_Value            = "_Value";
    const string s_MovementParams   = "_MovementParams";
    #endregion

    #region Formatters
    void ChangeParams<T>( string name, T[] values )
        => typeof( BugBehaviour )
                .GetField( name, BindingFlags.Static | BindingFlags.NonPublic )
                .SetValue( null, values );
    
    string FmtArray<T>( string name, T[] values )
    {
        StringBuilder result = new( $"static readonly {typeof( T ).Name}[] {name} = {{\n" );

        foreach ( var value in values )
        {
            result.Append( "    " );
            result.Append( value );
            switch ( typeof( T ).Name )
            {
                case "Single":
                case "System.Single":
                case "float":
                    result.Append( 'f' );
                    break;
            }
            result.Append( ",\n" );
        }
        result.Append( "};\n" );
        
        return result.ToString();
    }
    #endregion

    #region Params
    [Header( "Worm params" )]
    [SerializeField] float          _wormSpeed          = 0.25f;
    [SerializeField] uint           _wormValue          = 100;
                     MovementParams _wormMovement       = MovementParams.Default;

    [Header( "Grasshopper params" )]
    [SerializeField] float          _grassSpeed         = 0.50f;
    [SerializeField] uint           _grassValue         = 300;
                     MovementParams _grassMovement      = MovementParams.Default;

    [Header( "Butterfly params" )]
    [SerializeField] float          _butterSpeed        = 1f;
    [SerializeField] uint           _butterValue        = 600;
    [SerializeField] bool           _butterOscillate    = true;
    [SerializeField] float          _butterAmplitude    = 0.25f;
    [SerializeField] float          _butterDuration     = 1f;
                     MovementParams _butterMovement     = MovementParams.Oscillation( true, 0.25f, 1f );

    [Header( "Beetle params" )]
    [SerializeField] float          _beetleSpeed        = 2f;
    [SerializeField] uint           _beetleValue        = 1200;
    [SerializeField] bool           _beetleErraticMove  = true;
    [SerializeField] Vector3        _beetleSize         = BC_Vecs.Fill3( 0.8f );
    [SerializeField] float          _beetleDuration     = 1f;
                     MovementParams _beetleMovement     = MovementParams.ErraticBounds( true, BC_Vecs.Fill3( 0.8f ), 0.75f );

    [Header( "Bee params" )]
    [SerializeField] float          _beeSpeed           = 4f;
    [SerializeField] uint           _beeValue           = 10000;
    [SerializeField] bool           _beeErraticMove     = true;
    [SerializeField] Vector3        _beeSize            = Vector3.one;
    [SerializeField] float          _beeDuration        = 0.75f;
                     MovementParams _beeMovement        = MovementParams.ErraticBounds( true, Vector3.one, 0.75f );
    #endregion

    void Awake()
    {
        var ogCulture       = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        var customCulture   = (System.Globalization.CultureInfo)ogCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        Debug.LogWarning( 
            "[MovementParamsConfig] - This script changes BugBehavior's readonly params using reflection. "     +
            "It'll be automatically destroyed on release builds, so make sure to modify them on said script "   +
            "when you think you're good."
        );

        float[] speeds  = { _wormSpeed, _grassSpeed, _butterSpeed, _beetleSpeed, _beetleSpeed };
         uint[] values  = { _wormValue, _grassValue, _butterValue, _beetleValue, _beeValue };

        _wormMovement   = MovementParams.Default;
        _grassMovement  = MovementParams.Default;
        _butterMovement = MovementParams.Oscillation( _butterOscillate, _butterAmplitude, _butterDuration );
        _beetleMovement = MovementParams.ErraticBounds( _beetleErraticMove, _beetleSize, _beetleDuration ); ;
        _beeMovement    = MovementParams.ErraticBounds( _beeErraticMove, _beeSize, _beeDuration ); ;
        MovementParams[]
            moveParams  = { _wormMovement, _grassMovement, _butterMovement, _beetleMovement, _beeMovement };


        ChangeParams( s_Speed,          speeds );
        ChangeParams( s_Value,          values );
        ChangeParams( s_MovementParams, moveParams );

        StringBuilder output = new( "[MovementParamsConfig]\n" );
        output.Append( FmtArray( s_Speed, speeds ) );
        output.Append( '\n' );
        output.Append( FmtArray( s_Value, values ) );
        output.Append( '\n' );
        output.Append( $"static readonly MovementParams[] _MovementParams = {{\n" );
        
        string                              fmtDefault      = typeof( MovementParams ).Name + ".Default";
        Func<bool, float, float, string>    fmtOscillation  = ( o, a, d ) => typeof( MovementParams ).Name + $".Oscillation({o.ToString().ToLower()}, {a}f, {d}f)";
        Func<bool, Vector3, float, string>  fmtErratic      = ( m, v, d ) =>
        {
            string vs = v switch
            {
                var _ when v == Vector3.zero    => nameof( Vector3.zero ),
                var _ when v == Vector3.one     => nameof( Vector3.one  ),
                var _ when v.x == v.y
                        && v.x == v.z
                        && v.y == v.z           => $"{nameof( BC_Vecs )}.{nameof( BC_Vecs.Fill3 )}({v.x})",
                _                               => $"new Vector3({v.x}f, {v.y}f, {v.z}f)"
            };

            return typeof( MovementParams ).Name + $".ErraticBounds({m.ToString().ToLower()}, {vs}, {d}f)";
        };

        Action<string> fmtMovement = ( s ) =>
        {
            output.Append( "    " );
            output.Append( s );
            output.Append( ",\n" );
        };

        fmtMovement( fmtDefault );
        fmtMovement( fmtDefault );
        fmtMovement( fmtOscillation( _butterOscillate, _butterAmplitude, _butterDuration ) );
        fmtMovement( fmtErratic( _beetleErraticMove, _beetleSize, _beetleDuration ) );
        fmtMovement( fmtErratic( _beeErraticMove, _beeSize, _beeDuration ) );

        output.Append( "};\n" );
        Debug.Log( output.ToString() );
     
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
    }
#endif
}
