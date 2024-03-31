using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void OnSliderValueChanged(float value)
    {
        switch (audioCategory)
        {
            case AudioCategory.Master:
                AudioManager.Instance.SetMasterVolume(value);
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
