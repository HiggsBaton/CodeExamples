using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The audio manager
/// </summary>
public static class AudioManager
{
    static bool initialized = false;
    public static AudioSource audioSource;
    static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>
    public static bool Initialized
    {
        get { return initialized; }
    }

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        audioClips.Add(AudioClipName.BulletCollision, 
            Resources.Load<AudioClip>("impactPlate_heavy_000"));
        audioClips.Add(AudioClipName.LockCollision, 
            Resources.Load<AudioClip>("impactMetal_medium_000"));
        audioClips.Add(AudioClipName.BreakableCollision, 
            Resources.Load<AudioClip>("impactPlate_light_000"));
        audioClips.Add(AudioClipName.HatCollision, 
            Resources.Load<AudioClip>("impactBell_heavy_000"));
        audioClips.Add(AudioClipName.PrisonerFreed, 
            Resources.Load<AudioClip>("yay"));
    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void Play(AudioClipName name)
    {
        if (audioSource != null) audioSource.PlayOneShot(audioClips[name]);
    }

    public static void MusicOnOff()
    {
        if (audioSource != null) audioSource.volume = 0;
    }
}
