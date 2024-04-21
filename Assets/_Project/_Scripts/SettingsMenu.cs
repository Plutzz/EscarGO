using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : NetworkBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject Canvas;

    private LobbyAPI lobbyAPI;

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false); // Disable gameObject on spawn (because you can't spawn a disabled game object)
        lobbyAPI = GameObject.FindGameObjectWithTag("LobbyAPI").GetComponent<LobbyAPI>();

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

    public void LeaveLobby()
    {
        lobbyAPI.LeaveLobby();
        SceneManager.LoadSceneAsync("BenLobby");
    }
}
