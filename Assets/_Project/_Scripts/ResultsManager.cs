using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ResultsManager : NetworkSingleton<ResultsManager>
{
    [SerializeField] private TextMeshProUGUI playersReadyText;
    private int numPlayersReady = 0;
    private List<ulong> playerReadyIds = new List<ulong>();
    private Dictionary<int, PlayerAttributes> playerStats;
    [SerializeField] private GameObject playerResultsUIPrefab;
    [SerializeField] private Transform leaderboard;

    [SerializeField] private string playScene = "NetworkGameScene";

    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            Destroy(ScoringSingleton.Instance?.gameObject);
            return;
        }
        HandleStats();
        Debug.Log(playerStats[0]);
        DisplayPlayersReadyClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClients.Count);
    }

    private void HandleStats()
    {
        playerStats = ScoringSingleton.Instance.playerStats;
        Debug.Log("Destroy Scoring Singleton");
        Destroy(ScoringSingleton.Instance.gameObject);
        foreach(var player in playerStats)
        {
            DisplayPlayerStatsClientRpc(player.Value.username, player.Value.score);
        }
    }

    [ClientRpc]
    private void DisplayPlayerStatsClientRpc(string name, int score)
    {
        GameObject playerResults = Instantiate(playerResultsUIPrefab, leaderboard);
        playerResults.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        playerResults.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = score + "";
    }



    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(ulong _playerId)
    {
        if(playerReadyIds.Contains(_playerId))
        {
            return;
        }

        // If the player has not already readied up for another match,
        // add them to the ready list and update ui on all clients
        numPlayersReady++;
        playerReadyIds.Add(_playerId);
        DisplayPlayersReadyClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClients.Count);
        if (numPlayersReady == NetworkManager.Singleton.ConnectedClients.Count)
        {
            // Enable all player objects
            foreach (var _player in NetworkManager.Singleton.ConnectedClientsList)
            {
                _player.PlayerObject.gameObject.SetActive(true);
            }

            NetworkManager.Singleton.SceneManager.LoadScene(playScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    public void DisplayPlayersReadyClientRpc(int _numPlayersReady, int _numPlayersConnected)
    {
        playersReadyText.text = _numPlayersReady + " / " + _numPlayersConnected + "  Players Ready";
    }

}
