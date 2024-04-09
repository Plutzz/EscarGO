using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractableTeleport : InteractableSpace
{
    [SerializeField] private Vector3 teleportPosition;
    [SerializeField] private Vector3 teleportRotation;
    [SerializeField] private Transform[] teamTeleportPositions;
    [SerializeField] private bool teleportToTeamDoors;
    public override void Interact(PlayerInventory inventory)
    {
        Vector3 teleportPosition;
        Vector3 teleportRotation;
        if(teleportToTeamDoors)
        {
            int playerNumber = GetPlayerNumberServerRpc(NetworkManager.Singleton.LocalClientId);
            teleportPosition = teamTeleportPositions[playerNumber].position;
            teleportRotation = teamTeleportPositions[playerNumber].eulerAngles;
        }
        else
        {
            teleportPosition = this.teleportPosition;
            teleportRotation = this.teleportRotation;
        }

        // Teleport the player to the designated position
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().position = teleportPosition;

        inventory.GetComponentInChildren<FirstPersonCamera>().rotation.x = teleportRotation.y;
        inventory.transform.Find("Orientation").eulerAngles = teleportRotation;
    }

    [ServerRpc]
    private int GetPlayerNumberServerRpc(ulong clientId)
    {
        return ScoringSingleton.Instance.GetPlayerNumber(clientId);
    }
}
