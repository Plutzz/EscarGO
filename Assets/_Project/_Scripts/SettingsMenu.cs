using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SettingsMenu : NetworkBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject Canvas;
    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false); // Disable gameObject on spawn (because you can't spawn a disabled game object)

        if (!IsOwner)
        {
            Canvas.SetActive(false);
            enabled = false;
            return;
        }
    }

    public void OpenMenu()
    {
        inputManager?.SwitchActionMap("UI");
        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        inputManager?.SwitchActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }
}
