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
    [SerializeField] private float raycastLength;

    [SerializeField] private Item donut;

    private PlayerInventory playerInventory;
    private InputManager inputManager;
    [SerializeField] private RecipeBook recipeBook;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        playerInventory = GetComponent<PlayerInventory>();
        inputManager = GetComponent<InputManager>();
        recipeBook = GetComponentInChildren<RecipeBook>(true);
    }
    private void Update()
    {
        if (inputManager.InteractPressedThisFrame)
        {
            CheckForInteractable();
            CheckForTrash();
            CheckForCustomer();
        }

        //Change book pages
        if(Input.GetKeyDown(KeyCode.Q))
        {
            recipeBook.ChangePrevPage();
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            recipeBook.ChangeNextPage();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        RaycastHit[] hits = Physics.RaycastAll(ray, raycastLength, minigameLayer);
        foreach (var hit in hits)
        {
            InteractableSpace interactable = hit.collider.gameObject.GetComponentInChildren<InteractableSpace>();
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
            AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.ItemTrash, transform.position);
        }
        else
        {
        }
    }

    private void CheckForCustomer()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, orientation.forward, raycastLength, customerLayer);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                Customer customer = hit.collider.gameObject.GetComponent<Customer>();
                if (customer != null)
                {
                    if (customer.TryCompleteOrder(playerInventory))
                    {
                        return;
                    }
                }
            }
        }

        /*Collider[] customerColliders = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, customerLayer);


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
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position + orientation.forward * offset, radius);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * raycastLength);
    }
}
