using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkSingletonPersistent<AudioManager>
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume;
    [Range(0, 1)]
    public float musicVolume;
    [Range(0, 1)]
    public float ambienceVolume;
    [Range(0, 1)]
    public float SFXVolume;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;



    public void Start()
    {
        base.OnNetworkSpawn();
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        InitializeAmbience(FMODEvents.Instance.Ambience);
        InitializeMusic(FMODEvents.Instance.Music);

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        UpdateVolume();
    }

    private void UpdateVolume()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float)area);
    }

    public void PlayOneShot(FMODEvents.NetworkSFXName sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(FMODEvents.Instance.SfxArray[(int)sound], worldPos);
    }

    public EventInstance PlayLoopingSFX(FMODEvents.NetworkSFXName sound)
    {
        EventInstance sfxEventInstance = CreateInstance(FMODEvents.Instance.SfxArray[(int)sound]);
        sfxEventInstance.start();
        return sfxEventInstance;
    }
    
    // Asks the server to play a sfx on all clients *CLIENT AUTHORITATIVE
    [ServerRpc(RequireOwnership = false)]
    public void PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName sound, Vector3 worldPos)
    {
        PlayOneShotAllClientRpc(sound, worldPos);
    }

    // Plays a sfx on all clients
    [ClientRpc]
    private void PlayOneShotAllClientRpc(FMODEvents.NetworkSFXName sound, Vector3 worldPos)
    {
        PlayOneShot(sound, worldPos);
    }
    [ClientRpc]
    private void TestClientRpc(NetworkObjectReference networkObject)
    {

    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(FMODEvents.NetworkSFXName sound, GameObject emitterGameObject)
    {
        if (emitterGameObject.TryGetComponent(out StudioEventEmitter emitter))
        {

        }
        else
        {
            emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
        }
        emitter.EventReference = FMODEvents.Instance.SfxArray[(int)sound];
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    public override void OnDestroy()
    {
        CleanUp();
    }

    public enum MusicArea
    {
        GRAY_AREA = 0,
        BLUE_AREA = 1
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        UpdateVolume();
    }
}

