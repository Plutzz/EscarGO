using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyList : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateLobby()
    {
        string lobbyName = "Lobby name";
        int maxPlayers = 4;

        await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

        // Debug.Log("Create Lobby! " + lobby.name + " " + lobby.);
    }
}
