using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoringSingleton : NetworkSingleton<ScoringSingleton>
{
    private Dictionary<int, PlayerAttributes> playerStats = new Dictionary<int, PlayerAttributes>();
    public List<PlayerAttributes> alivePlayers = new List<PlayerAttributes>();


    public override void OnNetworkSpawn()
    {
        if(!IsServer) 
        {
            return; 
        }
        SendToSpectateClientRpc(false);
        ResetStrikesClientRpc();
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

    [ServerRpc(RequireOwnership = false)]
    public void RecieveStrikeServerRpc(int playerNumber)
    {
        if (!IsServer) return;

        if (playerStats.ContainsKey(playerNumber))
        {
            playerStats[playerNumber].strikes--;
            Debug.Log($"PLAYER: {playerNumber} has {playerStats[playerNumber].strikes} strikes remaining");
            // Client rpc to PLAYER SPECIFIC CLIENT and give a strike on UI
            RecieveStrikeClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { playerStats[playerNumber].clientId } } });

        }

        if (playerStats[playerNumber].strikes == 0) {
            Debug.Log("FIRED");

            // Set Player to Spectate mode
            SendToSpectateClientRpc(true, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { playerStats[playerNumber].clientId } } });
            // Despawn all of this player's customers

            alivePlayers.Remove(playerStats[playerNumber]);

            // If there is 1 player remaining, end the game
            if (alivePlayers.Count <= 1)
            {
                GameManager.Instance.EndGameServerRpc();
            }

        }
    }

    [ClientRpc]
    private void RecieveStrikeClientRpc(ClientRpcParams sendParams = default)
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Canvas>().GetComponentInChildren<StrikeUI>().RemoveStar();
    }

    [ClientRpc]
    private void ResetStrikesClientRpc()
    {
        // Hard coded to reset stars to three, might have to change if you want to have a different number of stars
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Canvas>().GetComponentInChildren<StrikeUI>().ResetStars(3);
    }

    // If true, sends player to spectate, if false takes player out of spectate
    [ClientRpc]
    private void SendToSpectateClientRpc(bool isSpectating, ClientRpcParams sendParams = default)
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Player>().Spectate(isSpectating);
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
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JumpSound, transform.position);
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

        int playerNumber = 0;
        // Assigns players a number 0 - 3
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerStats.Add(playerNumber, new PlayerAttributes());
            playerStats[playerNumber].clientId = player.ClientId;
            playerStats[playerNumber].playerNumber = playerNumber;
            alivePlayers.Add(playerStats[playerNumber]);
            playerNumber++;
        }
    }
    
}

[Serializable]
public class PlayerAttributes {
    public ulong clientId;
    public int playerNumber;
    public int score = 0;
    public int strikes = 3;
}
