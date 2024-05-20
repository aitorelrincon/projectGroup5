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

    static readonly MovementParams[] _MovementParams = 
    {
        MovementParams.Default,
        MovementParams.Default,
        MovementParams.Oscillation( true, 0.25f, 1f ),
        MovementParams.ErraticBounds( true, BC_Vecs.Fill3( 0.8f ), 1.00f ),
        MovementParams.ErraticBounds( true, Vector3.one, 0.75f )
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
    public float speed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Speed[(int)_kind]; }
    public uint  value { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Value[(int)_kind]; }
    public MovementParams movementParams
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get => _MovementParams[(int)_kind];
    }
    #endregion

    #region Unity methods
    void Start()
    {
        Assert.AreEqual( transform.childCount, 1 );
        Debug.Log( "{" + string.Join(", ", _Speed ) + "}" );
        Debug.Log( "{" + string.Join(", ", _Value ) + "}" );
        Debug.Log( "{" + string.Join(", ", _MovementParams ) + "}" );

        _bug    = transform.GetChild( 0 );
        _target = GameManager.Instance.player;
        
        _rb     = gameObject.GetOrAddComponent<Rigidbody>();
        {
            _rb.mass        = 500;
            _rb.useGravity  = movementParams.IsDefault;
         
            _rb.constraints |= RigidbodyConstraints.FreezeRotationX 
                            |  RigidbodyConstraints.FreezeRotationZ;
        }

        movementParams.Force( gameObject );
        var look    = gameObject.GetOrAddComponent<LookAtTarget>();
        look.target = _target;
    }

    void FixedUpdate()
    {
        var vel = ( _target.transform.position - transform.position ).normalized
                * ( speed * BASE_SPEED );
        
        _rb.velocity = vel;
    }
    #endregion
}
