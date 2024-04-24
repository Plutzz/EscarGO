using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyTrigger : NetworkBehaviour
{
    [SerializeField] private string SceneName;
    [SerializeField] private TextMeshPro LobbyText;
    [SerializeField] private LevelLoader levelLoader;
    private int numPlayersReady = 0;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            UpdateLobbyTextClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClientsList.Count);
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>().SetupNametagClientRpc(null);
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerReadyServerRpc();
        }
    }

    private IEnumerator FadeTransition()
    {
        levelLoader.fadeTransition();

        yield return new WaitForSeconds(1);

        NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc()
    {
        numPlayersReady++;
        UpdateLobbyTextClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClientsList.Count);

        if (numPlayersReady == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            Debug.Log("Loading Next Scene");
            FadeTransitionClientRPC();
            // levelLoader.fadeTransition();
            // NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    private void FadeTransitionClientRPC()
    {
        StartCoroutine(FadeTransition());
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerUnreadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerUnreadyServerRpc()
    {
        numPlayersReady--;
        UpdateLobbyTextClientRpc(numPlayersReady ,NetworkManager.Singleton.ConnectedClientsList.Count);
    }
    [ClientRpc]
    private void UpdateLobbyTextClientRpc(int _playersReady, int _playersConnected)
    {
        LobbyText.text = _playersReady + " / " + _playersConnected + " Players Ready";
    }



}
