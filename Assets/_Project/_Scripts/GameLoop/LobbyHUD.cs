using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHUD : MonoBehaviour
{
    [SerializeField] private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_Text lobbyCodeText;

    // Update is called once per frame
    void Update()
    {
        lobbyCodeText.text = lobbyAPI.GetLobbyCode();
    }
}
