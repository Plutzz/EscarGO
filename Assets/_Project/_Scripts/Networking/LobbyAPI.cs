using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor.Search;
using UnityEngine;

public class LobbyAPI : MonoBehaviour
{
    private float heartbeatTimeMax = 15f;
    private int lobbyCount = 25;

    private Lobby hostLobby;
    private float heartbeatTimer;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = heartbeatTimeMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    // Created lobby
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "Lobby name";
            int maxPlayers = 4;

            // Currently a var because lobby.cs is confusing the system
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            hostLobby = lobby;

            Debug.Log("Create Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Create list of lobbies
    private async void ListLobbies()
    {
        try 
        {
            // Set up rules for hte query lobby
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = lobbyCount,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            // Query Lobbies async is queriable in the parameters
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            List<Lobby> lobbies = queryResponse.Results;

            Debug.Log("Lobbies Found: " + lobbies.Count);
            foreach (Lobby lobby in lobbies)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }
}
