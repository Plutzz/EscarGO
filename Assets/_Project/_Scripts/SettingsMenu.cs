using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SettingsMenu : NetworkBehaviour
{
    [SerializeField] private InputManager inputManager;
    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false); // Disable gameObject on spawn (because you can't spawn a disabled game object)

        if (!IsOwner)
        {
            Destroy(gameObject);
            return;
        }
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
