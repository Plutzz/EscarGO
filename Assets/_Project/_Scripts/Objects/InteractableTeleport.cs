using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
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
    [SerializeField] private GameObject sunlight;
    public override void Interact(PlayerInventory inventory)
    {
        Vector3 teleportPosition;
        Vector3 teleportRotation;
        if(teleportToTeamDoors)
        {
            teleportPosition = teamTeleportPositions[lastDoor].position;
            teleportRotation = teamTeleportPositions[lastDoor].eulerAngles;
            sunlight.transform.eulerAngles = new Vector3(45, 0, 0);
        }
        else
        {
            backKitchenDoor.lastDoor = doorNumber;
            teleportPosition = this.teleportPosition;
            teleportRotation = this.teleportRotation;
            sunlight.transform.eulerAngles = new Vector3(89.5f, 0, 0);
        }

        // Teleport the player to the designated position
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().position = teleportPosition;
        //NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientNetworkTransform>().Teleport(teleportPosition, Quaternion.identity, NetworkManager.Singleton.LocalClient.PlayerObject.transform.localScale);
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.DoorClose, transform.position);
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.DoorClose, teleportPosition);
        inventory.GetComponentInChildren<FirstPersonCamera>().rotation.x = teleportRotation.y;
        inventory.transform.Find("Orientation").eulerAngles = teleportRotation;
    }

}
