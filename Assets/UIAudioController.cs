using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioController : MonoBehaviour
{
    public Slider _masterSlider, _musicSlider, _sfxSlider;

    public void ToggleMaster()
    {
        AudioSettings.Instance.ToggleMaster();
    }

    public void ToggleMusic()
    {
        AudioSettings.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioSettings.Instance.ToggleSFX();
    }

    public void MasterVolume()
    {
        AudioSettings.Instance.MasterVolume(_masterSlider.value);
    }

    public void MusicVolume()
    {
        AudioSettings.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioSettings.Instance.SFXVolume(_sfxSlider.value);
    }
}
