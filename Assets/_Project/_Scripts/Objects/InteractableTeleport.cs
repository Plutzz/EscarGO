using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

public class InteractableTeleport : InteractableSpace
{
    [SerializeField] private Vector3 teleportPosition;
    [SerializeField] private Vector3 teleportRotation;
    [SerializeField] private bool teleportToTeamDoors;
    [SerializeField] private Transform[] teamTeleportPositions;
    [SerializeField] private int doorNumber = 0;
    public int lastDoor = 0;
    [SerializeField] private InteractableTeleport backKitchenDoor;
    public override void Interact(PlayerInventory inventory)
    {
        Vector3 teleportPosition;
        Vector3 teleportRotation;
        if(teleportToTeamDoors)
        {
            teleportPosition = teamTeleportPositions[lastDoor].position;
            teleportRotation = teamTeleportPositions[lastDoor].eulerAngles;
        }
        else
        {
            backKitchenDoor.lastDoor = doorNumber;
            teleportPosition = this.teleportPosition;
            teleportRotation = this.teleportRotation;
        }

        // Teleport the player to the designated position
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().position = teleportPosition;
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.DoorClose, transform.position);
        inventory.GetComponentInChildren<FirstPersonCamera>().rotation.x = teleportRotation.y;
        inventory.transform.Find("Orientation").eulerAngles = teleportRotation;
    }

}
