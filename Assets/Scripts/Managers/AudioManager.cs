// When defined, PlayAudioClip will log a warning when the clip isn't correctly loaded.
#define PLAYAUDIO_LOG_MISSING_VALUE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

using BugCatcher.Utils;
using BugCatcher.Extensions;

/// <summary>
/// AudioManager singleton class.
/// </summary>
[DefaultExecutionOrder(-9999), DisallowMultipleComponent]
public class AudioManager : MonoSingle<AudioManager>
{
    public const int GEN_CHANNEL = 0;

    public const string PREFS_VOLMUS_KEY = "MusicVolume";
    public const string PREFS_VOLSFX_KEY = "SfxVolume";

    [Header("Audio Sources")]
    public AudioSource   musicSource;
    public AudioSource[] sfxChannels;
    public Dictionary<string, AudioClip> 
        sfxClips   = new(), 
        musicClips = new();

    [Header("Audio Volume")]
    [Range(0, 1)] float _musicVolume = 1.0f;
    [Range(0, 1)] float _sfxVolume   = 1.0f;      
    public float musicVolume { get => _musicVolume; set => _musicVolume = Mathf.Clamp( _musicVolume, 0.0f, 1.0f ); }
    public float sfxVolume   { get =>   _sfxVolume; set =>   _sfxVolume = Mathf.Clamp(   _sfxVolume, 0.0f, 1.0f ); }

    [Header("Spawned Audio Parents")]
    public bool selfDefaultParent = true;
    public Transform 
        defaultSfxParent   = null,
        defaultMusicParent = null;

    public void LoadPrefs()
    {
        if ( PlayerPrefs.HasKey( PREFS_VOLMUS_KEY ) )
            musicVolume = PlayerPrefs.GetFloat( PREFS_VOLMUS_KEY );

        if ( PlayerPrefs.HasKey( PREFS_VOLSFX_KEY ) )
            musicVolume = PlayerPrefs.GetFloat( PREFS_VOLSFX_KEY );

        musicSource.volume = musicVolume;
        foreach ( var c in sfxChannels )
            c.volume = sfxVolume;
    }

    protected override void OnAwake()
    {
        LoadPrefs();
        LoadSFXClips();
        LoadMusicClips();

        if ( selfDefaultParent )
        {
            if ( defaultSfxParent.IsMissingOrNone() ) 
                defaultSfxParent = transform;
            
            if ( defaultMusicParent.IsMissingOrNone() )
                defaultMusicParent = transform;
        }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private string NameOfDict( Dictionary<string, AudioClip> dict ) => dict switch
    {
        _ when ReferenceEquals( dict, sfxClips )   => nameof( sfxClips ),
        _ when ReferenceEquals( dict, musicClips ) => nameof( musicClips ),
        _ => "?[UNKNOWN]?"
    };

    /// <summary>
    /// Logs a warning message when one or more clips failed to load properly,
    /// indicating which ones failed
    /// </summary>
    /// <param name="audioClips">Dictionary to be checked</param>
    private bool VerifyLoading( Dictionary<string, AudioClip> audioClips )
    {
        var failedClips = audioClips.Where((kvp) => kvp.Value is null);
        int fClipCount = failedClips.Count();
        if ( fClipCount == 0 )
            return true;

        string dictName = NameOfDict(audioClips);

        StringBuilder sb = new(
            $"[AudioManager] - Failed to load {fClipCount} clips onto {dictName} with keys:\n"
        );

        foreach ( var kvp in failedClips )
        {
            sb.Append( kvp.Key );
            sb.Append( '\n' );
        }

        Debug.LogWarning( sb.ToString() );
        return false;
    }

    /// <summary>
    /// Loads SFX clips from Resources/SFX. Logs a warning if loading any clip fails.
    /// </summary>
    private void LoadSFXClips()
    {
        sfxClips["Example"] = Resources.Load<AudioClip>( "SFX/Example" );

        VerifyLoading( sfxClips );
    }

    /// <summary>
    /// Loads Music clips from Resources/Music. Logs a warning if loading any clip fails.
    /// </summary>
    private void LoadMusicClips()
    {
        musicClips["Example"] = Resources.Load<AudioClip>( "Music/Example" );

        VerifyLoading( musicClips );
    }

    /// <summary>
    /// Find and play audio with the passed clipName.
    /// Logs a warning when:
    /// - Passed clip dictionary cannot find the clip.
    /// - The clip isn't properly loaded.
    /// </summary>
    /// <param name="audioClips">Dictionary with audio clips</param>
    /// <param name="audioSource">Audio source to play the audio with</param>
    /// <param name="clipName">Clip name</param>
    /// <param name="loop">Determines if the audio will loop or not</param>
    private void PlayAudioClip( 
        Dictionary<string, AudioClip> audioClips,
        AudioSource                   audioSource,
        string                        clipName,
        bool                          loop = false )
    {
        string dictName = NameOfDict(audioClips);

        if ( !audioClips.ContainsKey( clipName ) )
        {
            Debug.LogWarning( $"[AudioManager] - AudioClip \"{clipName}\" not found within {dictName}." );
            return;
        }

        var clip = audioClips[clipName];
#if PLAYAUDIO_LOG_MISSING_VALUE
        if ( clip is null )
        {
            Debug.LogWarning( $"[AudioManager] - AudioClip \"{clipName}\" couldn't be properly loaded." );
            return;
        }
#endif

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
        Debug.Log( $"[AudioManager] - Now playing \"{clipName}\" {( loop ? "(looped)" : "" )})" );
    }

    /// <summary>
    /// Plays clip from sfxClips by name.
    /// </summary>
    /// <param name="clipName">SFX clip name</param>
    /// <param name="index">AudioSource channel index.</param>
    public void PlaySFX( string clipName, int index = GEN_CHANNEL )
        => PlayAudioClip( sfxClips, sfxChannels[index], clipName, false );

    public bool IsPlayingSFX( int index ) => sfxChannels[index].isPlaying;

    /// <summary>
    /// Plays clip from musicClips by name.
    /// </summary>
    /// <param name="clipName">SFX clip name</param>
    /// <param name="loop">Determines if the music will loop or not</param>
    public void PlayMusic( string clipName, bool loop = true )
        => PlayAudioClip( musicClips, musicSource, clipName, loop );

    private void SpawnClip(
        Dictionary<string, AudioClip> audioClips,
        string clipName,
        Vector3 position,
        Transform parent,
        float volume = 1.0f
    ) {
        var gameObject                = new GameObject( $"OneShotClip_{clipName}" );
        gameObject.transform.position = position;
        gameObject.transform.parent   = parent;

        var clip = audioClips[clipName];
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy( gameObject, clip.length * ( ( Time.timeScale < 0.01f ) ? 0.01f : Time.timeScale ) );
    }

    public void SpawnSFX( string clipName, Vector3 position, Transform parent )
        => SpawnClip( sfxClips, clipName, position, parent, sfxVolume );
    public void SpawnSFX( string clipName, Vector3 position )
        => SpawnClip( sfxClips, clipName, position, defaultSfxParent, sfxVolume );

    public void SpawnMusic( string clipName, Vector3 position, Transform parent )
        => SpawnClip( musicClips, clipName, position, parent, musicVolume );

    public void SpawnMusic( string clipName, Vector3 position )
        => SpawnClip( musicClips, clipName, position, defaultMusicParent, musicVolume );

    /// <summary>
    /// Stops sfxSource by index.
    /// </summary>
    /// <param name="index">AudioSource channel index.</param>
    public void StopSFX( int index ) => sfxChannels[index].Stop();

    /// <summary>
    /// Stops all SFX sources.
    /// </summary>
    /// <param name="index">AudioSource channel index.</param>
    public void StopAllSFX()
    {
        foreach ( var c in sfxChannels ) c.Stop();
    }

    /// <summary>
    /// Stops musicSource.
    /// </summary>
    public void StopMusic() => musicSource.Stop();
}