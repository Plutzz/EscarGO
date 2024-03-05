using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    public override void OnNetworkSpawn()
    {
        // If this is not called on the server, return
        if(!IsServer) return;

        Debug.Log("GameManager");

        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
        {
            var _player = NetworkManager.Singleton.ConnectedClientsIds[i];

            Debug.Log("Teleporting: " + _player);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { _player }
                }
            };

            teleportPlayersClientRpc(clientRpcParams);
        }
    }

    [ClientRpc]
    private void teleportPlayersClientRpc(ClientRpcParams clientRpcParams)
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Transform>();

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        //player.GetComponent<PlayerStateMachine>().playerInputActions.Disable();
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);
    }
}
