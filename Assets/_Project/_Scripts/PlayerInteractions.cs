using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteractions : MonoBehaviour
{
    [Header("Interactable Range")]
    [SerializeField] private float offset;
    [SerializeField] private float radius;
    [SerializeField] private Transform orientation;

    [SerializeField] private LayerMask interactables;
    [SerializeField] private LayerMask trashLayer;
    [SerializeField] private LayerMask minigameLayer;

    private bool inStation = false;
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
        if(Input.GetKeyDown(KeyCode.F))
        {
            CheckForStation();
        }
    }

    private void CheckForInteractable() {
        Collider[] interactableColliders = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, interactables);
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
            playerInventory.RemoveSelectedItem();
        }
        else {
            TipsManager.Instance.SetTip("No Trashcan here", 2f);
        }
    }

    private void CheckForStation()
    {

        Collider[] stations = Physics.OverlapSphere(transform.position + orientation.forward * offset, radius, minigameLayer);

        if(stations.Length == 0)
        {
            return;
        }
        
        SuperStation interactable = stations[0].gameObject.GetComponent<SuperStation>();

        if (interactable != null)
        {
            InputManager.Instance.playerInput.SwitchCurrentActionMap("MiniGames");
            Debug.Log("switched to minigame: " + gameObject);
            interactable.Activate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + orientation.forward * offset, radius);
    }
}
