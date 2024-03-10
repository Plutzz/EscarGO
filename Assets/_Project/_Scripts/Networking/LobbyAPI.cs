using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor.Search;
using UnityEngine;

public class LobbyAPI : MonoBehaviour
{
    private string lobbyName = "Lobby name";
    private int maxPlayers = 4;
    private float heartbeatTimeMax = 15f;
    private int lobbyCount = 25;

    private Lobby hostLobby;
    private float heartbeatTimer;

    private string playerName;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Emery" + Random.Range(10,99);
        Debug.Log(playerName);

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
            // Lobby options
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer()
            };

            // Currently a var because lobby.cs is confusing the system
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            hostLobby = lobby;
            
            Debug.Log("Create Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            Players(hostLobby);

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

    // Join lobby with code
    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log("Joined Lobby with code " + lobbyCode);

            Players(joinedLobby);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuickJoinLobby()
    {
        // Filters avaiable to pick specific maps and other stuff
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }

    private void Players(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name);
        foreach(var player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }
}
