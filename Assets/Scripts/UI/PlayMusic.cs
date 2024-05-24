using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] string _musicKey;

    void Start()
    {
        if ( AudioManager.Instance.musicSource.clip 
        !=   AudioManager.Instance.musicClips[_musicKey] )
             AudioManager.Instance.PlayMusic( _musicKey );
    }
}
