using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Assertions;

using BugCatcher.Extensions;
using System.Runtime.CompilerServices;
using System;
using OVR.OpenVR;
using BugCatcher.Extensions.Functional;

public class BugBehaviour : MonoBehaviour
{
    #region Constants
    public const float BASE_SPEED       = 100f;
    public const float LURK_WAIT_SECS   =  10f;
    public static readonly BugKind[] KindArray = (BugKind[])Enum.GetValues( typeof(BugKind) );
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

    public enum BugState
    {
        Approach,
        Lurk,
        Attack,
        Captured
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
    [SerializeField] ParticleSystem 
                        _particlesCaptured,
                        _particlesAttack;

    [Header("Particles")]
    #endregion

    #region Private variables
    Transform   _bug;
    Vector3     _target;
    BugState    _state;
    Rigidbody   _rb;
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
    void OnEnable()
    {
        SetState( BugState.Approach, GameManager.Instance.player.position );
    }

    void Start()
    {
        Assert.AreEqual( transform.childCount, 1 );
        // Debug.Log( "{" + string.Join(", ", _Speed ) + "}" );
        // Debug.Log( "{" + string.Join(", ", _Value ) + "}" );
        // Debug.Log( "{" + string.Join(", ", _MovementParams ) + "}" );

        _bug    = transform.GetChild( 0 );
        _rb     = gameObject.GetOrAddComponent<Rigidbody>();
        {
            _rb.mass        = 500;
            _rb.useGravity  = movementParams.IsDefault;
            // _rb.isKinematic = true;

            _rb.constraints |= RigidbodyConstraints.FreezeRotationX 
                            |  RigidbodyConstraints.FreezeRotationZ;
        }

        movementParams.Force( _bug.gameObject );
        var look    = gameObject.GetOrAddComponent<LookAtTarget>();
        // look.target = GameManager.Instance.player;
        look.target = GameManager.Instance.lookAt;
    }

    void FixedUpdate()
    {
        if ( _rb.useGravity )
            transform.position = transform.position.WithY( 0f );

        switch ( _state )
        {
            case BugState.Lurk:
                if ( Mathf.Approximately( (_target - transform.position).magnitude, 0f ) )
                {
                    var point = GameManager.Instance.lurkPoint;
                    if ( _rb.useGravity )
                        point.y = 0f;

                    _target = point;
                }
                goto case BugState.Approach;

            case BugState.Attack:
            case BugState.Approach:
                _rb.velocity = ( _target - transform.position ).normalized 
                             * ( speed * BASE_SPEED * Time.fixedDeltaTime );
                if ( _rb.useGravity )
                    _rb.velocity = _rb.velocity.WithY( 0f );
                    // _rb.MovePosition( _target * ( Time.fixedDeltaTime * speed * BASE_SPEED ) );
                break;
        }
    }

    void OnTriggerEnter( Collider other )
    {
        switch ( _state, other.gameObject.layer )
        {
            // If it was approaching, lurk around for a bit
            case ( BugState.Approach, Layers.LurkZone ):
                SetState( BugState.Lurk, GameManager.Instance.lurkPoint );
                StartCoroutine( Lurk() );
                break;

            // BugState.Attack is a failsafe to make sure the player doesn't get hurt
            // until we want to, nothing more
            case ( BugState.Attack,   Layers.Player      ):
                GameManager.Instance.health -= 20;
                AudioManager.Instance.SpawnSFX( "BugBite", _bug.position );
                gameObject.SetActive( false );
                break;

            // Catch the bug
            case (               _,   Layers.Net         ):
                GameManager.Instance.AddScore( value );
                AudioManager.Instance.SpawnSFX( "BugCaught", _bug.position );
                _state = BugState.Captured;
                gameObject.SetActive( false );
                break;
        }
    }

    void OnDisable()
    {
        bool captured = _state == BugState.Captured,
             attack   = _state == BugState.Attack;

        if ( !captured && !attack )
            return;

        ParticleSystem particles = captured ? _particlesCaptured : _particlesAttack;
        var p = Instantiate(
            particles,
            _bug.transform.position, 
            Quaternion.Euler( 0, -90, -90 ) 
        );

        p.Play();
        Destroy( p.gameObject, particles.main.duration * Time.timeScale );
    }
    #endregion

    void SetState( BugState state, Vector3? target )
    {
        //  Debug.Log( state );
        _state = state;
        if ( target.HasValue )
            _target = target.Value;
    }

    IEnumerator Lurk()
    {
        yield return new WaitForSeconds( LURK_WAIT_SECS );
        SetState( BugState.Attack, GameManager.Instance.player.position );
    }
}
