using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Sound[] sounds;

    public Sound[] master, music, sfx;
    public AudioSource masterSource, musicSource, sfxSource;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    public void PlayMaster (string name)
    {
        Sound s = Array.Find(master, x => x.name == name);
        if(s == null)
        {
            Debug.Log ("Sound not found");
        }
        else
        {
            masterSource.clip = s.clip;
            masterSource.Play();
        }
    }

    public void PlayMusic (string name)
    {
        Sound s = Array.Find(music, x => x.name == name);
        if(s == null)
        {
            Debug.Log ("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX (string name)
    {
        Sound s = Array.Find(sfx, x => x.name == name);
        if(s == null)
        {
            Debug.Log ("Sound not found");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }

    public void ToggleMaster()
    {
        masterSource.mute = !masterSource.mute;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MasterVolume(float volume)
    {
        masterSource.volume = volume;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
