using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramerateSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public Dropdown framerateDropdown;

    private void Start()
    {
        InitializeDropdownOptions();
        
        framerateDropdown.onValueChanged.AddListener(OnFramerateChanged);
    }

    private void InitializeDropdownOptions()
    {
        framerateDropdown.options.Clear();
        framerateDropdown.options.Add(new Dropdown.OptionData("60fps"));
        framerateDropdown.options.Add(new Dropdown.OptionData("120fps"));
        framerateDropdown.options.Add(new Dropdown.OptionData("144fps"));
        framerateDropdown.options.Add(new Dropdown.OptionData("240fps"));
        framerateDropdown.options.Add(new Dropdown.OptionData("Uncapped"));
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
