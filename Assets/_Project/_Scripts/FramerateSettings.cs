using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FramerateSettings : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Dropdown framerateDropdown;

    private void Start()
    {

        if (framerateDropdown != null)
        {
            framerateDropdown.onValueChanged.AddListener(OnFramerateChanged);
        }
        else
        {
            Debug.LogError("Framerate Dropdown not found on GameObject: " + gameObject.name);
            Debug.Log("GameObject name searched for Dropdown: " + gameObject.name);
        }

        
    }


    private void OnFramerateChanged(int index)
    {
        switch (index)
        {
            case 0: // 60fps
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = 60;
                break;
            case 1: // 120fps
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 120;
                break;
            case 2: // 144fps
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 144;
                break;
            case 3: // 240fps
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 240;
                break;
            case 4: // Uncapped
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
                break;
        }
    }
}
