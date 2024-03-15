using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractableTeleport : InteractableSpace
{
    [SerializeField] private Vector3 teleportPosition;
    [SerializeField] private Vector3 teleportRotation;
    public override void Interact(PlayerInventory inventory)
    {
        // Teleport the player to the designated position
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().position = teleportPosition;

        inventory.GetComponentInChildren<FirstPersonCamera>().rotation.x = teleportRotation.y;
        inventory.transform.Find("Orientation").eulerAngles = teleportRotation;
    }
}
