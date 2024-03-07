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
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRpc()
    {
        EndGameClientRpc();
        // Disable all player objects
        foreach (var _player in NetworkManager.Singleton.ConnectedClientsList)
        {
            _player.PlayerObject.gameObject.SetActive(false);
        }
        gameObject.GetComponent<ResultsUI>().GameComplete();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
    }

    private void teleportPlayers()
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Transform>();

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        //player.GetComponent<PlayerStateMachine>().playerInputActions.Disable();
        _player.GetComponent<InputManager>().SwitchActionMap("UI");
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Debug.Log("Teleport: " + spawnPos);
        _player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);
        _player.GetComponent<InputManager>().SwitchActionMap("Player");
    }



}
