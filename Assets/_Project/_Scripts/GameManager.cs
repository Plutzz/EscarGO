using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private TextMeshProUGUI donutText;
    private int donutsDelivered = 0;
    public override void OnNetworkSpawn()
    {
        // If this is not called on the server, return
        if(!IsServer) return;

        StartGameClientRpc();
    }
    private void teleportPlayers()
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Transform>();

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        //player.GetComponent<PlayerStateMachine>().playerInputActions.Disable();
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);
        _player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddDonutServerRpc()
    {
        if (!IsServer) return;

        Debug.Log("AddDonut");
        AddDonutClientRpc();
        if (donutsDelivered > 4)
        {
            //EndGame();
        }
    }

    [ClientRpc]
    private void AddDonutClientRpc()
    {
        Debug.Log("AddDonutClientRPC");
        donutsDelivered++;
        donutText.text = donutsDelivered + " Donuts Delivered";
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        teleportPlayers();
        NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRpc()
    {
        EndGameClientRpc();
        gameObject.GetComponent<ResultsUI>().GameComplete();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.SetActive(false);
    }



}
