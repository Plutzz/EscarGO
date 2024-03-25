using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioController : MonoBehaviour
{
    public Slider _masterSlider, _musicSlider, _sfxSlider;

    public void ToggleMaster()
    {
        AudioSystem.Instance.ToggleMaster();
    }

    public void ToggleMusic()
    {
        AudioSystem.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioSystem.Instance.ToggleSFX();
    }

    public void MasterVolume()
    {
        AudioSystem.Instance.MasterVolume(_masterSlider.value);
    }

    public void MusicVolume()
    {
        AudioSystem.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioSystem.Instance.SFXVolume(_sfxSlider.value);
    }
}
