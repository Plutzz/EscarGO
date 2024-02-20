using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableSpace : MonoBehaviour
{

    public abstract void Interact(PlayerInventory inventory);
}
