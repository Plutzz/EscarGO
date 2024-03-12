using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;


public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyAPI lobbyAPI;

    [SerializeField] private GameObject lobbyHolder;
    [SerializeField] private GameObject lobbyItemUI;
    
    [SerializeField] private GameObject hostGameUI;

    public void Start()
    {
        lobbyAPI.LobbiesUpdated += UpdateLobbyListClient;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        lobbyAPI.LobbiesUpdated -= UpdateLobbyListClient;
    }

    public void HostGameUI()
    {
        hostGameUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void StartMatch()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void UpdateLobbyListClient(List<Lobby> lobbies)
    {
        // this foreach feels a bit costly
        foreach (Transform child in lobbyHolder.transform)
        {
            if (child.CompareTag("LobbyItem"))
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Lobby lobby in lobbies)
        {
            BuildLobbyItem(lobby.Name, lobby.Data["Map"].Value, lobby.Data["Gamemode"].Value, lobby.Players.Count, lobby.MaxPlayers);
        }
    }

    private void BuildLobbyItem(string lobbyName, string mapName, string gamemode, int currPlayers, int maxPlayers)
    {
        GameObject currLobbyItem = Instantiate(lobbyItemUI, lobbyHolder.transform);

        LobbyUIItem lobbyItem = currLobbyItem.GetComponent<LobbyUIItem>();

        lobbyItem.lobbyName = lobbyName;
        lobbyItem.mapName = mapName;
        lobbyItem.gamemode = gamemode;
        lobbyItem.currentPlayers = currPlayers;
        lobbyItem.maxPlayers = maxPlayers;
    }
}
