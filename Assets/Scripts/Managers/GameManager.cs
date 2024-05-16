using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using BugCatcher.Extensions;
using BugCatcher.Utils;
using System.Diagnostics;

public class GameManager : MonoShared<GameManager>
{
    [SerializeField] TMP_Text _timeTmp, _scoreTmp;
    
    uint    _currentScore = 0;
    Timer   _timer;
    char[]  _timeFmt = new char[ 5 ];

    void Start()
    {
#if false
        _timeTmp    = GetComponentInChildren<TMP_Text>();
        _scoreTmp   = GetComponentInChildren<TMP_Text>();
#else
#endif

        _timer           = this.GetOrAddComponent<Timer>();
        _timer.CountMode = Timer.Count.Up;

        if (Camera.main != null)
        {
            // Places the text elements in front of the camera for now
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }

    void Update()
    {
#if false
        _timer.TryFormatMinutes( _timeFmt );
        _timeTmp.text = _timeFmt.ToString();
#endif
    }

    public void AddScore(uint scoreToAdd)
    {
        _currentScore   += scoreToAdd;
        _scoreTmp.text  = "Score: " + _currentScore;
    }
}