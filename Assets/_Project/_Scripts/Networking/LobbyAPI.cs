using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyAPI : MonoBehaviour
{
    public event Action<List<Lobby>> LobbiesUpdated;

    // private string lobbyName = "Lobby name";
    // private int maxPlayers = 4;
    private float heartbeatTimeMax = 15f;
    private float lobbyUpdateTimeMax = 1.1f;
    private int lobbyCount = 25;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;

    private string playerName;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Emery" + UnityEngine.Random.Range(10,99);
        Debug.Log(playerName);

    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollUpdate();
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

    private async void HandleLobbyPollUpdate()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;

            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = lobbyUpdateTimeMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    // Created lobby
    public async void CreateLobby(string lobbyName, 
                                  int maxPlayers, 
                                  bool isPriv, 
                                  string mapName,
                                  string gamemode)
    {
        try
        {
            // Lobby options
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPriv,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    // Map, string name, and S1 allows us to filter later on
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, mapName, DataObject.IndexOptions.S1)},
                    {"Gamemode", new DataObject(DataObject.VisibilityOptions.Public, gamemode, DataObject.IndexOptions.S2)},
                }
            };

            // Currently a var because lobby.cs is confusing the system
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;
            
            Debug.Log("Create Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            Players(hostLobby);

            ListLobbies();

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
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["Map"].Value);
            }

            if (LobbiesUpdated != null)
            {
                LobbiesUpdated.Invoke(lobbies);
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
            

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby with code " + lobbyCode);

            Players(lobby);
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
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["Map"].Value);
        foreach(var player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void UpdateLobbyMap(string map)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions 
            {
                Data = new Dictionary<string, DataObject> 
                {
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, map)}
                }
            });
            joinedLobby = hostLobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}

                }
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void KickPlayer(string playerID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerID);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions 
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Getter methods
    public async Task<List<Lobby>> GetLobbiesAsync()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            List<Lobby> lobbies = queryResponse.Results;
            return lobbies;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
}
