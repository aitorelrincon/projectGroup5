// #define TEXT_IMPLEMENTED

using UnityEngine;
using TMPro;

using BugCatcher.Utils;
using BugCatcher.Extensions;
using BugCatcher.Extensions.Functional;

public class GameManager : MonoShared<GameManager>
{
    [SerializeField] TMP_Text   _timeTmp, _scoreTmp;
    [SerializeField] Transform  _player;
    [SerializeField] Transform  _lookAt;
    [SerializeField] Collider   _lurkZone;
    [SerializeField] Collider   _playerCollider;

    uint    _currentScore = 0;
    Timer   _timer;
    char[]  _timeFmt = new char[ 5 ];

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
    }

    public void AddScore(uint scoreToAdd)
    {
        _currentScore   += scoreToAdd;
#if TEXT_IMPLEMENTED
        _scoreTmp.text  = "Score: " + _currentScore;
#endif
    }
}