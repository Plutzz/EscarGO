using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;


public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_Text lobbyListText;

    public void Start()
    {
        lobbyAPI.LobbiesUpdated += UpdateLobbyListText;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        lobbyAPI.LobbiesUpdated -= UpdateLobbyListText;
    }

    public void StartMatch()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void UpdateLobbyListText(List<Lobby> lobbies)
    {
        string lobbyList = "Lobbies:\n";

        foreach (Lobby lobby in lobbies)
        {
            lobbyList += lobby.Name + "\n";
        }

        lobbyListText.text = lobbyList;
    }
}
