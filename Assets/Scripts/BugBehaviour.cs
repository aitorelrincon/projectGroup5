using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Assertions;

using BugCatcher.Extensions;
using System.Runtime.CompilerServices;
using System;

public class BugBehaviour : MonoBehaviour
{
    #region Constants
    public const float BASE_SPEED = 2;
    #endregion

    #region Nested types
    public enum BugKind
    {
        Worm,
        Grasshopper,
        Butterfly,
        Beetle,
        Bee
    }

    [System.Serializable]
    public struct OscillationParams
    {
        public bool   enabled;
        public float  amplitude;
        public float  duration;

        public static readonly OscillationParams Original = new( true, 0.25f, 1.0f );
        public static readonly OscillationParams Disabled = default;

        public OscillationParams( bool enabled, float amplitude, float duration )
        {
            this.enabled    = enabled;
            this.amplitude  = amplitude;
            this.duration   = duration;
        }
    }
    #endregion

    #region Bug stats
    static readonly float[] _Speed = 
    {
        0.25f,
        0.50f,
        1.00f,
        2.00f,
        4.00f,
    };

    static readonly uint[] _Value = 
    {
        100,
        300,
        600,
        1200,
        10000,
    };

    static readonly OscillationParams[] _OscillationParams = 
    {
        OscillationParams.Disabled,
        OscillationParams.Disabled,
        OscillationParams.Original,
        OscillationParams.Original,
        OscillationParams.Original
    };
    #endregion

    #region Bug config
    [Header("Bug config")] 
    [SerializeField] BugKind _kind;
    #endregion

    #region Private variables
    Transform   _bug;
    Transform   _target;
    Rigidbody   _rb;
    Oscillation _osc;
    #endregion

    #region Properties
    public float Speed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Speed[(int)_kind]; }
    public uint  Value { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Value[(int)_kind]; }
    public OscillationParams OscillationParam
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get => _OscillationParams[(int)_kind];
    }
    #endregion

    #region Unity methods
    void Start()
    {
        Assert.AreEqual( transform.childCount, 1 );
        
        _bug    = transform.GetChild( 0 );
        _target = GameManager.Instance.player;
        
        _rb     = gameObject.GetOrAddComponent<Rigidbody>();
        {
            _rb.mass        = 500;
            _rb.useGravity  = !OscillationParam.enabled;
         
            _rb.constraints |= RigidbodyConstraints.FreezeRotationX 
                            |  RigidbodyConstraints.FreezeRotationZ;
        }

        _osc    = _bug.GetOrAddComponent<Oscillation>();
        {
            _osc.enabled   = OscillationParam.enabled;
            _osc.oscillate = OscillationParam.enabled;
            _osc.amplitude = OscillationParam.amplitude;
            _osc.duration  = OscillationParam.duration;
        }

        var look    = gameObject.GetOrAddComponent<LookAtTarget>();
        look.target = _target;
    }

    void FixedUpdate()
    {
        var vel = ( _target.transform.position - transform.position ).normalized
                * ( Speed * BASE_SPEED );
        
        _rb.velocity = vel;
    }
    #endregion
}
