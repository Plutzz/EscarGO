using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostGameUI : MonoBehaviour
{
    private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_Dropdown maxPlayers;
    [SerializeField] private Toggle privateLobby;
    [SerializeField] private TMP_Dropdown map;
    [SerializeField] private TMP_Dropdown gameMd;
    [SerializeField] private GameObject lobbyGameUI;


    // This exists for testing
    [SerializeField] private GameObject lobbyUI;

    private void Start()
    {
        lobbyAPI = LobbyAPI.Instance;
    }
    public void HostLobby()
    {
        lobbyAPI.CreateLobby(lobbyName.text, 
                             int.Parse(maxPlayers.options[maxPlayers.value].text),
                             privateLobby.isOn,
                             map.options[map.value].text,
                             gameMd.options[gameMd.value].text);

        lobbyName.text = "";
        this.gameObject.SetActive(false);
        lobbyGameUI.SetActive(true);
    }

    public void BackLobby()
    {
        lobbyUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
