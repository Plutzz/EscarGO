using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoringSingleton : NetworkSingleton<ScoringSingleton>
{
    private Dictionary<ulong, PlayerAttributes> playerScores = new Dictionary<ulong, PlayerAttributes>();

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int scoreChange, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        var clientId = serverRpcParams.Receive.SenderClientId;

        if (playerScores.ContainsKey(clientId))
        {
            playerScores[clientId].score += scoreChange;
        }
        else {
            playerScores.Add(clientId, new PlayerAttributes());
            playerScores[clientId].score = scoreChange;
        }

        FindWinningPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetStrikeServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        var clientId = serverRpcParams.Receive.SenderClientId;

        if (playerScores.ContainsKey(clientId))
        {
            playerScores[clientId].strikes--;
        }
        else
        {
            playerScores.Add(clientId, new PlayerAttributes());
            playerScores[clientId].strikes --;
        }

        if (playerScores[clientId].strikes <= 0) {
            Debug.Log("FIRED");
        }
    }

    private void FindWinningPlayer() {

        ulong winningPlayerID = 0;
        int winningScore = -1;

        foreach (var player in playerScores) {
            if (player.Value.score > winningScore) { 
                winningScore = player.Value.score;
                winningPlayerID = player.Key;
            }
        }

        UpdatePlayersClientRpc(winningPlayerID);
    }

    [ClientRpc]
    public void UpdatePlayersClientRpc(ulong firstPlacePlayer, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == firstPlacePlayer)
        {
            Debug.Log("You are winning");
        }
        else {
            Debug.Log($"Player: {firstPlacePlayer} is winning");
        }
    }
    
}

public class PlayerAttributes {
    public int score;
    public int strikes = 3;

    public float reputation;
}
