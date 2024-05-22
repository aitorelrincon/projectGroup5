using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class AudioMuteController : MonoBehaviour
{
    public AudioSource audioSource;   // Reference to the AudioSource component
    public Interactable checkbox;     // Reference to the MRTK Interactable (Checkbox)
    public AudioClip audioClip;       // Reference to the AudioClip

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;  // Assign the audio clip to the audio source
            audioSource.Play();            // Optionally start playing the audio
        }

        if (checkbox != null)
        {
            // Register for the OnClick event
            checkbox.OnClick.AddListener(ToggleMute);
        }
    }

    private void ToggleMute()
    {
        if (audioSource != null)
        {
            // Toggle the mute state based on the checkbox
            audioSource.mute = checkbox.IsToggled;
        }
    }

    private void OnDestroy()
    {
        if (checkbox != null)
        {
            // Unregister the OnClick event to prevent memory leaks
            checkbox.OnClick.RemoveListener(ToggleMute);
        }
    }
}
