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
    [SerializeField] private GameObject lobbyGameUI;

    [SerializeField] private TMP_InputField joinCodeInput;

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
        lobbyGameUI.SetActive(true);
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
            BuildLobbyItem(lobby.Name, lobby.Data["Map"].Value, lobby.Data["Gamemode"].Value, lobby.Players.Count, lobby.MaxPlayers, lobby.Id);
            Debug.Log(lobby.Name);
            Debug.Log(lobby.Id);
            Debug.Log(lobby.LobbyCode);
        }
    }

    public void JoinWithCode()
    {
        string joinCode = joinCodeInput.text;

        lobbyAPI.JoinLobbyByCode(joinCode);

        if (lobbyAPI.GetJoinedLobby() != null)
        {
            joinCodeInput.text = "";
            this.gameObject.SetActive(false);
            lobbyGameUI.SetActive(true);
        }
        else
        {
            // Any other logic here, perhaps error message
            Debug.Log("Lobby NOT FOUND");
        }
    }

    private void BuildLobbyItem(string lobbyName, string mapName, string gamemode, int currPlayers, int maxPlayers, string lobbyID)
    {
        GameObject currLobbyItem = Instantiate(lobbyItemUI, lobbyHolder.transform);

        LobbyUIItem lobbyItem = currLobbyItem.GetComponent<LobbyUIItem>();

        lobbyItem.lobbyName = lobbyName;
        lobbyItem.mapName = mapName;
        lobbyItem.gamemode = gamemode;
        lobbyItem.currentPlayers = currPlayers;
        lobbyItem.maxPlayers = maxPlayers;
        lobbyItem.lobbyID = lobbyID;
        Debug.Log("Lobby ID: " + lobbyID);

        lobbyItem.lobbyAPI = lobbyAPI;
        lobbyItem.lobbyUI = this.gameObject;
        lobbyItem.lobbyGameUI = lobbyGameUI;
    }
}
