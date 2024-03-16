using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private InputManager inputManager;
    private void Start()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().pauseMenu = this;
        inputManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>();
        gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        inputManager.SwitchActionMap("UI");
        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        inputManager.SwitchActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }
}
