using System;
using UnityEngine;

public static class AudioSourceExtensions 
{
    public static event Action<AudioSource> OnAudioStateChanged = delegate { };

    public static void PlayWithEvent(this AudioSource audioSource) 
    {
        audioSource.Play();
        OnAudioStateChanged(audioSource);
    }

    public static void StopWithEvent(this AudioSource audioSource) 
    {
        audioSource.Stop();
        OnAudioStateChanged(audioSource);
    }
}
