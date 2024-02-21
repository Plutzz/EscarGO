using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    public string itemName;

    public virtual void Interact() {}
}
