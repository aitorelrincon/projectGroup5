using UnityEngine;
using BugCatcher.Utils;
using BugCatcher.Extensions;

public class ErraticBounds : MonoBehaviour
{
    public bool     move     = true;
    public Vector3  size     = Vector3.one;
    public float    duration = 1f;
    Bounds          _bounds;
    Vector3         _velocity;
    Vector3         _startPos, _nextPos;
    float           _startT;

    void Start()
    {
        if ( duration <= 0.65f )
            Debug.LogWarning( "[ErraticBounds] - Durations <= 0.65 result in incorrect behaviour" );

        _bounds = new( transform.localPosition, size );
        _startPos   = _bounds.center;
        _nextPos    = BC_Random.InsideBounds( _bounds );
        _startT     = Time.time;
    }

    void Update()
    {
        if ( !move ) return;
        
        var t = Mathf.PingPong( Time.time - _startT, duration );

        transform.localPosition =
#if SMOOTHSTEP
            Vector3.SmoothDamp( transform.localPosition, _nextPos, ref _velocity, t );
#else
            Vector3.Lerp( transform.localPosition, _nextPos, Mathf.SmoothStep(0f, duration, t) );
#endif

        if ( Mathf.Approximately( ( _nextPos - transform.localPosition ).magnitude, 0 ) )
        {
            _startPos   = _nextPos;
            _nextPos    = BC_Random.InsideBounds( _bounds );
            _velocity   = Vector3.zero;
            _startT     = Time.time;
        }
    }
}
