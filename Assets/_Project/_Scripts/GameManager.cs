using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private Vector3 spawnPos;
    public override void OnNetworkSpawn()
    {
        // If this is not called on the server, return
        if(!IsServer) return;

        ScoringSingleton.Instance.AssignPlayerNumbers();
        teleportPlayersClientRpc();
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        Cursor.lockState = CursorLockMode.Locked;

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
        Cursor.lockState = CursorLockMode.Confined;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().scoreText.text = "0 Points";
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>().ClearInventory();
    }

    [ClientRpc]
    private void teleportPlayersClientRpc()
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.transform;

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _player.GetComponent<Rigidbody>().position = spawnPos;
        //_player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);

    }



}
