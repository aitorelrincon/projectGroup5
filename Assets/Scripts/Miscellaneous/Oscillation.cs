using UnityEngine;
using BugCatcher.Extensions;

public class Oscillation : MonoBehaviour
{
    [Header("Oscillation Config")]
    public bool oscillate = true;
    public float amplitude = 0.25f;
    public float duration = 1;
    float _oscStartT, _startY, _oscMin, _oscMax;

    void Start()
    {
        _oscMin = -amplitude;
        _oscMax = amplitude;
        _oscStartT = Time.time;
        _startY = transform.position.y;
    }

    void Update()
    {
        if ( !oscillate ) return;

        var t = Mathf.PingPong( Time.time - _oscStartT, duration );

        transform.position =
            transform.position.WithY(
                _startY + Mathf.SmoothStep( _oscMin, _oscMax, t )
            );

        if ( Mathf.Approximately( transform.position.y, _oscMax ) )
        {
            _oscMin *= -1;
            _oscMax *= -1;
            _oscStartT = Time.time;
        }
    }
}
