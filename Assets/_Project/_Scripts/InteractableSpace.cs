using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InteractableSpace : NetworkBehaviour
{
    public abstract void Interact(PlayerInventory inventory);
}
