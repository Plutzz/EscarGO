using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ScoringSingleton : NetworkSingletonPersistent<ScoringSingleton>
{
    public Dictionary<int, PlayerAttributes> playerStats { get; private set; }
    public List<PlayerAttributes> alivePlayers = new List<PlayerAttributes>();


    protected override void Awake()
    {
        base.Awake();
        ResetPlayerStats();
    }

    public void ResetPlayerStats()
    {
        playerStats = new Dictionary<int, PlayerAttributes>();
        alivePlayers.Clear();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int playerNumber, int scoreChange)
    {
        if (!IsServer) return;

        if (playerStats.ContainsKey(playerNumber))
        {
            playerStats[playerNumber].score += scoreChange;
        }
        UpdatePlayerScoreClientRPC(playerStats[playerNumber].score, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { playerStats[playerNumber].clientId } } });
        FindWinningPlayer();
    }

    private void FindWinningPlayer() {

        int winningPlayerNumber = 0;
        int winningScore = -1;

        foreach (var player in playerStats) {
            if (player.Value.score > winningScore) {
                winningScore = player.Value.score;
                winningPlayerNumber = player.Key;
            }
        }

        //UpdatePlayersClientRpc(winningPlayerNumber);
    }

    [ClientRpc]
    private void UpdatePlayerScoreClientRPC(int score, ClientRpcParams clientRpcParams)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.CompleteOrder, transform.position);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().scoreText.text = score + " Points";
    }

    //[ClientRpc]
    //public void UpdatePlayersClientRpc(int firstPlacePlayer)
    //{
    //    if (NetworkManager.Singleton.LocalClientId == playerStats[firstPlacePlayer].clientId)
    //    {
    //        Debug.Log("You are winning");
    //    }
    //    else {
    //        Debug.Log($"Player: {firstPlacePlayer} is winning");
    //    }
    //}

    // Assigns players a number for keeping track of customers, side of cafe, etc.
    public void AssignPlayerNumbers()
    {
        if (!IsServer) return;
        Debug.Log("AssignPlayerNumbers");
        int playerNumber = 0;
        // Assigns players a number 0 - 3
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerStats.Add(playerNumber, new PlayerAttributes());
            playerStats[playerNumber].clientId = player.ClientId;
            playerStats[playerNumber].playerNumber = playerNumber;

            if (playerStats[playerNumber].username == null && AuthenticationService.Instance.IsSignedIn)
            {
                SetupPlayerNameClientRpc(playerNumber, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { playerStats[playerNumber].clientId } } });
            }
            alivePlayers.Add(playerStats[playerNumber]);
            playerNumber++;
        }
    }
    [ClientRpc]
    private void SetupPlayerNameClientRpc(int playerNumber, ClientRpcParams rpcParams)
    {
        string playerName = AuthenticationService.Instance.PlayerName;
        SetPlayerNameServerRpc(playerName, playerNumber);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, int playerNumber)
    {
        playerStats[playerNumber].username = playerName.Substring(0, playerName.Length - 5);
        NetworkManager.Singleton.ConnectedClients[playerStats[playerNumber].clientId].PlayerObject
            .GetComponent<Player>().SetupNametagClientRpc(playerStats[playerNumber].username);
    }

    public int GetPlayerNumber(ulong clientId)
    {
        foreach (var player in playerStats)
        {
            if(player.Value.clientId == clientId)
            {
                return player.Value.playerNumber;
            }
        }
        
        return 0;
    }

    public Dictionary<int, PlayerAttributes> GetPlayerStats()
    {
        return playerStats;
    }

}

[Serializable]
public class PlayerAttributes {
    public string username;
    public ulong clientId;
    public int playerNumber;
    public int score = 0;
}
