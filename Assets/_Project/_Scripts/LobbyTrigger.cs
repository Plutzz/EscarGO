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
    [SerializeField] private TMP_Text LobbyCountdown;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private float countdown = 3f;
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

    private IEnumerator Countdown()
    {
        float currentCountdown = countdown;

        while (currentCountdown >= 0)
        {
            if (numPlayersReady != NetworkManager.Singleton.ConnectedClientsList.Count)
            {
                LobbyCountdown.text = "";
                yield break;
            }

            LobbyCountdown.text = currentCountdown.ToString();
            yield return new WaitForSeconds(1);
            currentCountdown--;
        }

        LobbyCountdown.text = "";

        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        levelLoader.fadeTransition();

        yield return new WaitForSeconds(1);

        AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Level);
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc()
    {
        numPlayersReady++;
        UpdateLobbyTextClientRpc(numPlayersReady, NetworkManager.Singleton.ConnectedClientsList.Count);

        if (numPlayersReady == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            Debug.Log("Loading Next Scene");

            CountdownClientRPC();
            // FadeTransitionClientRPC();
            // levelLoader.fadeTransition();
            // NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    private void CountdownClientRPC()
    {
        StartCoroutine(Countdown());
    }

    // [ClientRpc]
    // private void FadeTransitionClientRPC()
    // {
    //     StartCoroutine(FadeTransition());
    // }

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
