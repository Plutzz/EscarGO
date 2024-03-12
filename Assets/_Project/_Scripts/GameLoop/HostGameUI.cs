using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostGameUI : MonoBehaviour
{
    [SerializeField] private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_InputField maxPlayers;
    [SerializeField] private Toggle privateLobby;
    [SerializeField] private TMP_Dropdown map;
    [SerializeField] private TMP_Dropdown gameMd;

    // This exists for testing
    [SerializeField] private GameObject lobbyUI;

    public void HostLobby()
    {
        lobbyAPI.CreateLobby(lobbyName.text, 
                             int.Parse(maxPlayers.text),
                             privateLobby.isOn,
                             map.options[map.value].text,
                             gameMd.options[gameMd.value].text);
    }

    public void LobbyUI()
    {
        lobbyUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
