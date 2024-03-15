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

    [SerializeField] private string playScene = "NetworkGameScene";

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        DisplayPlayersReadyClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClients.Count);
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
