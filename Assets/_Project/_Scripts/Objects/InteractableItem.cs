using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    public string itemName;
    public Rigidbody rb;
    public Collider col;

    public virtual void Interact(Transform hand)
    {
        Debug.Log("Interacting with " + itemName);
        if (hand.childCount > 0) // If hand is already holding something, detach it and attach new item
        {
            hand.GetChild(0).GetComponent<InteractableItem>().DetachFromHand();
        }
        AttachToHand(hand);
    }

    public virtual void DetachFromHand()
    {
        Debug.Log("Detaching " + itemName);
        rb.constraints = RigidbodyConstraints.None;
        col.enabled = true;
        transform.SetParent(null);
    }

    protected void AttachToHand(Transform hand)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        col.enabled = false;
        transform.SetParent(hand);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
