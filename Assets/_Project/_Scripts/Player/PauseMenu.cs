using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Temp Debug Variables")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    public bool ResumeButtonClicked = false;

    void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        resumeButton.onClick.AddListener(OnButtonClicked);
    }

    void Update()
    {
        if (InputManager.Instance.PausePressedThisFrame && pauseMenu != null || ResumeButtonClicked)
        {
            if (!pauseMenu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                pauseMenu.SetActive(true);
                InputManager.Instance.SwitchActionMap("UI");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.SetActive(false);
                InputManager.Instance.SwitchActionMap("Player");
            }

            ResumeButtonClicked = false;
        }
    }


    public void OnButtonClicked()
    {
        ResumeButtonClicked = true;
    }
}
