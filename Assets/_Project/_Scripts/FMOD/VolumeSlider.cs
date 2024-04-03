using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class VolumeSlider : MonoBehaviour
{
    
    public enum AudioCategory
    {
        Master,
        Music,
        SFX
    }

    public AudioCategory audioCategory;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        //Load the save value
        //AudioManager.Instance.SetMasterVolume(savedValue);
    }

    private void OnSliderValueChanged(float value)
    {
        switch (audioCategory)
        {
            
            case AudioCategory.Master:
                AudioManager.Instance.SetMasterVolume(value);
                // Save the value using PlayerPrefs
                break;
            case AudioCategory.Music:
                AudioManager.Instance.SetMusicVolume(value);
                break;
            case AudioCategory.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
        }
    }
}
