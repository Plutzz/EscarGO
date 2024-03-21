using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractions : NetworkBehaviour
{
    [Header("Interactable Range")]
    [SerializeField] private float offset;
    [SerializeField] private float radius;
    [SerializeField] private Transform orientation;

    [SerializeField] private LayerMask interactables;
    [SerializeField] private LayerMask trashLayer;
    [SerializeField] private LayerMask minigameLayer;
    [SerializeField] private LayerMask customerLayer;

    [SerializeField] private Item donut;

    private PlayerInventory playerInventory;
    private InputManager inputManager;



    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        playerInventory = GetComponent<PlayerInventory>();
        inputManager = GetComponent<InputManager>();
    }
    private void Update()
    {
        if (inputManager.InteractPressedThisFrame) { 
            CheckForInteractable();
        }
        if (Input.GetMouseButtonDown(1)) { 
            CheckForTrash();
            CheckForCustomer();
        }
    }

    private void CheckForInteractable() {
        Collider[] interactableColliders = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, minigameLayer);
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
        Collider[] trashColliders = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, trashLayer);
        if (trashColliders.Length > 0)
        {
            playerInventory.RemoveActiveItem();
        }
        else {
            TipsManager.Instance.SetTip("No Trashcan here", 2f);
        }
    }

    private void CheckForCustomer()
    {
        Collider[] customerColliders = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, customerLayer);

        if (customerColliders.Length > 0)
        {
            foreach (Collider col in customerColliders)
            {
                Customer customer = col.gameObject.GetComponent<Customer>();
                if (customer != null)
                {
                    // If a customer is found don't check any other customers
                    if(customer.TryCompleteOrder(playerInventory))
                    {
                        return;
                    }
                }

            }
        }
        else
        {
            TipsManager.Instance.SetTip("No Server here", 2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + orientation.forward * offset, radius);
    }
}
