using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoringSingleton : NetworkSingleton<ScoringSingleton>
{
    private Dictionary<int, PlayerAttributes> playerStats = new Dictionary<int, PlayerAttributes>();
    public List<int> alivePlayers = new List<int>();

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int playerNumber, int scoreChange)
    {
        if (!IsServer) return;

        if (playerStats.ContainsKey(playerNumber))
        {
            playerStats[playerNumber].score += scoreChange;
        }

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
        }

        if (playerStats[playerNumber].strikes == 0) {
            Debug.Log("FIRED");

            // Set Player to Spectate mode
            // Despawn all of this player's customers

            alivePlayers.Remove(playerNumber);

            // If there is 1 player remaining, end the game
            if (alivePlayers.Count <= 1)
            {
                GameManager.Instance.EndGameServerRpc();
            }

        }
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

        UpdatePlayersClientRpc(winningPlayerNumber);
    }

    [ClientRpc]
    public void UpdatePlayersClientRpc(int firstPlacePlayer)
    {
        if (NetworkManager.Singleton.LocalClientId == playerStats[firstPlacePlayer].clientId)
        {
            Debug.Log("You are winning");
        }
        else {
            Debug.Log($"Player: {firstPlacePlayer} is winning");
        }
    }

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
            alivePlayers.Add(playerNumber);
            playerNumber++;
        }
    }
    
}

public class PlayerAttributes {
    public ulong clientId;
    public int score = 0;
    public int strikes = 3;
}
