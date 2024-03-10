using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractableTeleport : InteractableSpace
{
    [SerializeField] private Vector3 teleportPosition;
    public override void Interact(PlayerInventory inventory)
    {
        // Teleport the player to the designated position
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().position = teleportPosition;
    }
}
