using UnityEngine.Audio;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Singleton declaration, used by other scripts
    public static AudioManager S;

    // Global volume settings, floats in [0.0, 1.0]
    public float masterVolume, musicVolume, sfxVolume;

    // List of all sounds played in the game
    public Sound[] sounds;

    void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * masterVolume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.isMusic)
                s.source.volume *= musicVolume;
            else
                s.source.volume *= sfxVolume;
        }
    }

    public void Play(string name)
    {
        Debug.Log("Playing sound clip: " + name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    /* Specialized functions called by other scripts */
    public void PlayTitleMusic()
    {
        // TODO: Make this visible in the editor
        // TEMP: This is not the final choice for title music (placeholder)
        Play("Wedding Dance");
    }

    public void StopTitleMusic()
    {
        Pause("Wedding Dance");
    }

    /* Functions called by the sliders in the volume settings */
    public void AdjustVolumeMaster(Slider slider)
    {
        masterVolume = slider.value;
        UpdateVolume();
    }

    public void AdjustVolumeMusic(Slider slider)
    {
        musicVolume = slider.value;
        UpdateVolume();
    }

    public void AdjustVolumeSFX(Slider slider)
    {
        sfxVolume = slider.value;
        UpdateVolume();
        // TODO: Should we trigger an sfx sound for the player to check against?
    }

    private void UpdateVolume()
    {
        // The volume of each sound is scaled by corresponding global volume settings
        // Master volume is applied to all sounds, music volume to music, sfx volume to non-music
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * masterVolume;

            if (s.isMusic)
                s.source.volume *= musicVolume;
            else
                s.source.volume *= sfxVolume;
        }
    }
}