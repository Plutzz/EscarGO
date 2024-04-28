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

    private string PlayerPrefsKey
    {
        get
        {
            return audioCategory.ToString() + "Volume";
        }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        //Load the save value
        float savedValue = PlayerPrefs.GetFloat(PlayerPrefsKey, 1f);
        slider.value = savedValue;

        OnSliderValueChanged(savedValue);
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

        PlayerPrefs.SetFloat(PlayerPrefsKey, value);
        PlayerPrefs.Save(); 
    }
}
