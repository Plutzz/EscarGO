using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteractions : MonoBehaviour
{
    [Header("Interactable Range")]
    [SerializeField] private float offset;
    [SerializeField] private float radius;

    [SerializeField] private LayerMask interactables;
    [SerializeField] private LayerMask trashLayer;

    private PlayerInventory playerInventory;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { 
            CheckForInteractable();
        }
        if (Input.GetMouseButtonDown(1)) { 
            CheckForTrash();
        }
    }

    private void CheckForInteractable() {
        Collider[] interactableColliders = Physics.OverlapSphere(transform.position + transform.forward * offset, radius, interactables);
        foreach (Collider col in interactableColliders) { 
            InteractableSpace interactable = col.gameObject.GetComponent<InteractableSpace>();
            if (interactable != null)
            {
                interactable.Interact(playerInventory);
            }
        }
    }

    private void CheckForTrash()
    {
        Collider[] trashColliders = Physics.OverlapSphere(transform.position + transform.forward * offset, radius, trashLayer);
        if (trashColliders.Length > 0)
        {
            playerInventory.RemoveSelectedItem();
        }
        else {
            TipsManager.Instance.SetTip("No Trashcan here", 2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * offset, radius);
    }
}