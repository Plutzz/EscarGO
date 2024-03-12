using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIItem : MonoBehaviour
{
    public string lobbyName = "";
    public string mapName = "";
    public int currentPlayers = 0;
    public int maxPlayers = 0;

    [SerializeField] private TMP_Text lobbyNameUI;
    [SerializeField] private TMP_Text mapNameUI;
    [SerializeField] private TMP_Text playerCountUI;

    // Update is called once per frame
    void Update()
    {
        lobbyNameUI.text = lobbyName;
        mapNameUI.text = mapName;
        playerCountUI.text = currentPlayers + "/" + maxPlayers;
    }
}
