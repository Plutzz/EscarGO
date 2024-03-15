using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIItem : MonoBehaviour
{
    public string lobbyName = "";
    public string mapName = "";
    public string gamemode = "";
    public int currentPlayers = 0;
    public int maxPlayers = 0;
    public string lobbyID = "";
    public LobbyAPI lobbyAPI;

    // UI Elements to disable
    public GameObject lobbyUI;
    public GameObject lobbyGameUI;

    [SerializeField] private TMP_Text lobbyNameUI;
    [SerializeField] private TMP_Text mapNameUI;
    [SerializeField] private TMP_Text gamemodeUI;
    [SerializeField] private TMP_Text playerCountUI;

    // Update is called once per frame
    void Update()
    {
        lobbyNameUI.text = lobbyName;
        mapNameUI.text = mapName;
        gamemodeUI.text = gamemode;
        playerCountUI.text = currentPlayers + "/" + maxPlayers;
    }

    public void JoinGame()
    {
        lobbyAPI.JoinLobbyById(lobbyID);

        lobbyUI.SetActive(false);
        lobbyGameUI.SetActive(true);
    }
}
