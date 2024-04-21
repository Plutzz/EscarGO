using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHUD : MonoBehaviour
{
    private LobbyAPI lobbyAPI;

    [SerializeField] private TMP_Text lobbyCodeText;

    private void Start()
    {
        lobbyAPI = LobbyAPI.Instance;
    }
    void Update()
    {
        lobbyCodeText.text = lobbyAPI.GetLobbyCode();
    }
}
