// #define TEXT_IMPLEMENTED

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using BugCatcher.Utils;
using BugCatcher.Extensions;
using BugCatcher.Extensions.Functional;

public class GameManager : MonoShared<GameManager>
{
    #region Nested types
    [System.Serializable]
    public struct WaveParams
    {
        [Min( 0 )] public int count;
        [SerializeField, Range( 0, 1 )] float _worm;
        [SerializeField, Range( 0, 1 )] float _grasshopper;
        [SerializeField, Range( 0, 1 )] float _butterfly;
        [SerializeField, Range( 0, 1 )] float _beetle;
        [SerializeField, Range( 0, 1 )] float _bee;

        public float worm { get => _worm; set => _worm = Mathf.Clamp01( value ); }
        public float grasshopper { get => _grasshopper; set => _grasshopper = Mathf.Clamp01( value ); }
        public float butterfly { get => _butterfly; set => _butterfly = Mathf.Clamp01( value ); }
        public float beetle { get => _beetle; set => _beetle = Mathf.Clamp01( value ); }
        public float bee { get => _bee; set => _bee = Mathf.Clamp01( value ); }

        public float this[BugBehaviour.BugKind kind] => kind switch
        {
            BugBehaviour.BugKind.Worm => worm,
            BugBehaviour.BugKind.Grasshopper => grasshopper,
            BugBehaviour.BugKind.Butterfly => butterfly,
            BugBehaviour.BugKind.Beetle => beetle,
            BugBehaviour.BugKind.Bee => bee,
            _ => throw new System.NotImplementedException()
        };

        public WaveParams( int c, float w, float g, float bf, float bt, float b )
        {
            count = c;
            _worm = Mathf.Clamp01( w );
            _grasshopper = Mathf.Clamp01( g );
            _butterfly = Mathf.Clamp01( bf );
            _beetle = Mathf.Clamp01( bt );
            _bee = Mathf.Clamp01( b );
        }
    }
    #endregion

    #region GameManager config
    [SerializeField] TMP_Text   _timeTmp, _scoreTmp;
    [SerializeField] Slider     _healthBar;
    [SerializeField] Transform  _player;
    [SerializeField] Transform  _lookAt;
    [SerializeField] Collider   _lurkZone;
    [SerializeField] Collider   _playerCollider;
    public WaveParams[] _waveParams = {
        new( 10, 1.00f, 1.00f, 0.50f, 0.00f, 0.00f ),
        new( 12, 0.25f, 0.75f, 1.00f, 0.25f, 0.00f ),
        new( 25, 0.25f, 0.50f, 1.00f, 1.00f, 0.05f ),
    };
    #endregion

    #region Private variables
    float           _health = 100f;
    uint            _currentScore   = 0;
    
    Timer           _timer;
    char[] _timeFmt = new char[ 5 ];
    
    MultiSpawner    _spawner;
    int             _totalCaught;
    int             _waveIdx;
    int             _waveCaught;
    #endregion

    #region Properties
    WaveParams currentWave { get => _waveParams[Mathf.Min( _waveIdx, _waveParams.Length - 1 )]; }
    public float health { get => _health; set => _health = Mathf.Clamp( value, 0, 100 ).Tee( h => _healthBar.value = h ); }
    public bool onGoing { get => _health > 0f && _timer.Secs > 0f; }
    #endregion

    public Transform player         { get => _player; }
    public Transform lookAt         { get => _lookAt; }
    public Vector3   lurkPoint   { 
        get => BC_Random
            .InsideBounds( _lurkZone.bounds )
            .Map( (p) => {
                if ( _playerCollider.bounds.Contains( p ) )
                {
                    var h = _lurkZone.bounds.extents;
                    if ( p.x < p.z )
                        p.x = p.x < h.x ? _playerCollider.bounds.min.x : _playerCollider.bounds.max.x;
                    else
                        p.z = p.z < h.z ? _playerCollider.bounds.min.z : _playerCollider.bounds.max.z;
                }

                return p;
            }); 
    }

    protected override void OnAwake()
    {
#if TEXT_IMPLEMENTED
        _timeTmp    = GetComponentInChildren<TMP_Text>();
        _scoreTmp   = GetComponentInChildren<TMP_Text>();
#endif

        _timer           = gameObject.GetOrAddComponent<Timer>();
        _timer.CountMode = Timer.Count.Down;
        _timer.Secs      = 120; // Two minutes

        _spawner         = GetComponent<MultiSpawner>();
        _spawner.proceedSpawnCheck = () => _spawner.spawnedCount + _waveCaught < currentWave.count;

#if false
        if (Camera.main != null)
        {
            // Places the text elements in front of the camera for now
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
#endif

        _player         = Camera.main.transform;
        if ( _playerCollider.IsMissingOrNone() )
            _player.GetComponent<Collider>();

        _lurkZone.gameObject.layer = Layers.LurkZone;
    }

    void Update()
    {
#if TEXT_IMPLEMENTED
        _timer.TryFormatMinutes( _timeFmt );
        _timeTmp.text = _timeFmt.ToString();
#endif
        if ( !onGoing ) return;

        if ( currentWave.count <= _waveCaught )
        {
            _waveCaught = 0;
            ++_waveIdx;
            foreach ( var k in BugBehaviour.KindArray )
                if ( !_spawner.TrySetProportion( (int)k, currentWave[k] ) )
                    Debug.LogError( "[GameManager] - Failed to update MultiSpawner proportions, index out of range" );

            // Avoids starting Wave3 again
            if ( _waveIdx <= 3 )
                AudioManager.Instance.PlayMusic( "Wave" + Mathf.Max( _waveIdx, 3 ) );
        }
    }

    public void AddScore(uint scoreToAdd)
    {
        _currentScore   += scoreToAdd;
#if TEXT_IMPLEMENTED
        _scoreTmp.text  = "Score: " + _currentScore;
#endif
    }
}