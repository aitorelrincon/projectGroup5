using System;
using System.Runtime.InteropServices;
using UnityEngine;

using BugCatcher.Extensions;
using BugCatcher.Extensions.Functional;
using System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Explicit)]
public struct MovementParams
{
    #region Nested types
    public enum MovementKind : byte { Default = 0, Oscillation, ErraticBounds }
    
    // Defining nested structs to keep explicit layout declaration simple
    struct Default_t { }
    struct Oscillation_t { 
        public bool     oscillate; 
        public float    amplitude, duration;  
    }

    struct ErraticBounds_t { 
        public bool     move;
        public Vector3  size;
        public float    duration;
    }
    #endregion

    #region Properties
    public MovementKind Kind            { get => _kind; }
    public bool         IsDefault       { get => _kind == MovementKind.Default; }
    public bool         IsOscillation   { get => _kind == MovementKind.Oscillation; }
    public bool         IsErraticBounds { get => _kind == MovementKind.ErraticBounds; }
    #endregion

    [FieldOffset(0)] MovementKind       _kind;
    [FieldOffset(1)] Default_t          _default;
    [FieldOffset(1)] Oscillation_t      _oscillation;
    [FieldOffset(1)] ErraticBounds_t    _erraticBounds;

    public static MovementParams Default 
        => default;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static MovementParams Oscillation( bool oscillate, float amplitude, float duration )
        => new() {
            _kind           = MovementKind.Oscillation,
            _oscillation    = new() { oscillate = oscillate, amplitude = amplitude, duration = duration },
        };

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static MovementParams ErraticBounds( bool move, Vector3 size, float duration ) 
        => new() {
            _kind           = MovementKind.ErraticBounds,
            _erraticBounds  = new() { move = move, size = size, duration = duration },
        };

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryApply( GameObject gameObject )
        => TryApply( gameObject, _kind );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryApply( GameObject gameObject, MovementKind k )
    {
        switch ( k )
        {
            case MovementKind.Default:
                return TryApplyDefault( gameObject );

            case MovementKind.Oscillation:
                return gameObject.TryGetComponent<Oscillation>( out var oscillation )
                    && TryApply( oscillation );
            
            case MovementKind.ErraticBounds:
                return gameObject.TryGetComponent<ErraticBounds>( out var erratic )
                    && TryApply( erratic );

            default:
                Debug.LogWarning( "Unimplemented MovementKind" );
                return false;
        }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryApplyDefault( GameObject gameObject )
    {
        if ( _kind != MovementKind.Default )
            return false;

        if ( gameObject.TryGetComponent<Oscillation>( out var osc ) )
            osc.enabled = false;

        if ( gameObject.TryGetComponent<ErraticBounds>( out var eb ) )
            eb.enabled  = false;

        return true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryApply( Oscillation component )
    {
        if ( _kind != MovementKind.Oscillation )
            return false;
        
        ApplyUnchecked( component );
        return true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryApply( ErraticBounds component )
    {
        if ( _kind != MovementKind.ErraticBounds )
            return false;

        ApplyUnchecked( component );
        return true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Force( GameObject gameObject )
    {
        switch ( _kind )
        {
            case MovementKind.Default:
                ApplyDefaultUnchecked( gameObject );
                return;

            case MovementKind.Oscillation:
                ApplyUnchecked( gameObject.GetOrAddComponent<Oscillation>() );
                return;

            case MovementKind.ErraticBounds:
                ApplyUnchecked( gameObject.GetOrAddComponent<ErraticBounds>() );
                return;

            default:
                Debug.LogWarning( "Unimplemented MovementKind" );
                return;
        }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public Oscillation Apply( Oscillation component )
    {
        if ( _kind != MovementKind.Oscillation )
            throw new ArgumentException( "MovementKind != Oscillation" );

        ApplyUnchecked( component );
        return component;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ErraticBounds Apply( ErraticBounds component )
    {
        if ( _kind != MovementKind.ErraticBounds )
            throw new ArgumentException( "MovementKind != ErraticBounds" );

        ApplyUnchecked( component );
        return component;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    void ApplyDefaultUnchecked( GameObject gameObject )
    {
        if ( gameObject.TryGetComponent<Oscillation>( out var osc ) )
            osc.enabled = false;

        if ( gameObject.TryGetComponent<ErraticBounds>( out var eb ) )
            eb.enabled = false;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    void ApplyUnchecked( Oscillation component )
    {
        if ( component.gameObject.TryGetComponent<ErraticBounds>( out var eb ) )
            eb.enabled = false;

        component.enabled   = true;
        component.oscillate = _oscillation.oscillate;
        component.amplitude = _oscillation.amplitude;
        component.duration  = _oscillation.duration;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    void ApplyUnchecked( ErraticBounds component )
    {
        if ( component.gameObject.TryGetComponent<Oscillation>( out var osc ) )
            osc.enabled = false;
        
        component.enabled   = true;
        component.move      = _erraticBounds.move;
        component.size      = _erraticBounds.size;
        component.duration  = _oscillation.duration;
    }

    public override string ToString() => $"{ _kind }";
}
