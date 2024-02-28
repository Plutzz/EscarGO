using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Temp Debug Variables")]
    [SerializeField] private GameObject pauseMenu;
    private PlayerInput playerInput;

    void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        playerInput = GetComponent<PlayerInput>();

    }

    void Update()
    {
        if (InputManager.Instance.PausePressedThisFrame && pauseMenu != null)
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
        }
    }
}
