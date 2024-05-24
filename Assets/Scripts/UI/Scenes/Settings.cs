using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("Music settings")]
    [SerializeField] PinchSlider    _musVolSlider;
    [SerializeField] Interactable   _musMuteCheckbox;

    [Header("SFX settings")]
    [SerializeField] PinchSlider    _sfxVolSlider;
    [SerializeField] Interactable   _sfxMuteCheckbox;

    [Header("Save & Return settings")]
    [SerializeField] Interactable   _saveButton;

    void Start()
    {
        // There should be no way for users to get here
        // with outdate AudioManager values so I'm gonna skip
        // loading prefs
        // AudioManager.Instance.LoadPrefs() );
        
        _musVolSlider.SliderValue   = AudioManager.Instance.musicVolume;
        _sfxVolSlider.SliderValue   = AudioManager.Instance.sfxVolume;
        _musMuteCheckbox.IsToggled  = AudioManager.Instance.musicMute;
        _sfxMuteCheckbox.IsToggled  = AudioManager.Instance.sfxMute;

        _musVolSlider.OnValueUpdated.AddListener( ctx => AudioManager.Instance.musicVolume = ctx.NewValue );
        _sfxVolSlider.OnValueUpdated.AddListener( ctx => {
            AudioManager.Instance.sfxVolume = ctx.NewValue;
            var src = AudioManager.Instance.sfxChannels[AudioManager.TSFX_CHANNEL];
            if ( !src.isPlaying 
            ||    src.time >= src.clip.length - 1.5f )
                AudioManager.Instance.PlaySFX( "BugBite", AudioManager.TSFX_CHANNEL );
        } );
        _musMuteCheckbox.OnClick.AddListener( () => AudioManager.Instance.musicMute = _musMuteCheckbox.IsToggled );
        _sfxMuteCheckbox.OnClick.AddListener( () => AudioManager.Instance.sfxMute   = _sfxMuteCheckbox.IsToggled );
    
        _saveButton.OnClick.AddListener( () => AudioManager.Instance.SavePrefs() );
    }
}
