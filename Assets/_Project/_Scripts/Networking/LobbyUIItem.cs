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

    public TMP_Text lobbyNameUI;
    public TMP_Text mapNameUI;
    public TMP_Text playerCountUI;

    // Update is called once per frame
    void Update()
    {
        lobbyNameUI.text = lobbyName;
        mapNameUI.text = mapName;
        playerCountUI.text = currentPlayers + "/" + maxPlayers;
    }
}
