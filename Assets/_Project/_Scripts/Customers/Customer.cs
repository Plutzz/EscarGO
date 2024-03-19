using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Customer : NetworkBehaviour
{
    [Header("References")]
    private Criteria criteria;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject orderPrefab;
    [SerializeField] private Vector3 orderOffset = new Vector3(0f, 1f, 0f);

    [Header("Timer")]
    private float timer;
    [SerializeField] private GameObject timerObjectPrefab;
    private GameObject timerObject;
    [SerializeField] private float patienceTime = 30f; // Time in seconds until customer leaves

    [Header("Seating Variables")]
    public int assignedPlayer;
    [SerializeField] private float interactionDistance = 2f;
    private bool orderReceived;
    private bool hasOrder = false;
    private Chair currentChair;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Select a random index within the bounds of the recipes array
        int randomIndex = Random.Range(0, CustomerSpawner.Instance.recipes.Length);

        GetCustomerOrderClientRpc(randomIndex);

        timer = patienceTime;
        orderReceived = false;
        timerObject = Instantiate(timerObjectPrefab, transform);
    }

    void Update()
    {
        if (!IsServer) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            LeaveServerRpc();
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= interactionDistance && hasOrder)
        {
            orderReceived = true;
            //patienceFillObject.SetActive(true); // Show the patience fill object when order is received
        }

        UpdateTimerScale();

        if (orderReceived)
        {
            Exit();
        }

    }
    [ClientRpc]
    private void GetCustomerOrderClientRpc(int _index)
    {
        // Instantiate a new GameObject for the order sprite
        GameObject orderObject = Instantiate(orderPrefab, transform);

        // Position the order sprite above the customer
        orderObject.transform.localPosition = orderOffset;

        // Add a SpriteRenderer component to the order GameObject
        Material itemMaterial = Instantiate(orderObject.GetComponent<Renderer>().material);
        orderObject.GetComponent<Renderer>().material = itemMaterial;

        // Set the order sprite to the item's sprite
        criteria = Instantiate(CustomerSpawner.Instance.recipes[_index]);

        if (criteria != null && criteria.objectPairs[0].item.itemSprite != null)
        {
            itemMaterial.SetTexture("_Texture", criteria.objectPairs[0].item.itemSprite.texture);
        }
        else
        {
            Debug.LogWarning("Order item or its sprite is not assigned!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveServerRpc()
    {
        LeaveClientRpc();
        CustomerSpawner.Instance.customerCount--;
        ScoringSingleton.Instance.RecieveStrikeServerRpc(assignedPlayer);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FufillOrderServerRpc()
    {
        LeaveClientRpc();
        CustomerSpawner.Instance.customerCount--;
        ScoringSingleton.Instance.AddScoreServerRpc(assignedPlayer, criteria.score);
    }

    
    [ClientRpc]
    public void LeaveClientRpc()
    {
        Destroy(gameObject);
    }
    void UpdateTimerScale()
    {
        float scale = Mathf.Clamp01(1 - timer / patienceTime) / 85;
        float xScale = scale * 7/2;
        timerObject.transform.localScale = new Vector3(xScale, scale, 1f);
    }

    public void Leave()
    {
        Debug.Log("You take too long! I'm out");
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; 
        }
        gameObject.SetActive(false); 
    }

    public void Exit()
    {
        Debug.Log("Thank you!");
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; 
        }
        gameObject.transform.position = CustomerSpawner.Instance.transform.position;
        gameObject.SetActive(false);
    }

    public void TryCompleteOrder(PlayerInventory inventory)
    {
        if (inventory.CurrentlyHasItem())
        {

            foreach (Criteria.Required criteriaItem in criteria.objectPairs)
            {

                if (inventory.getCurrentItem().itemName == criteriaItem.item.itemName)
                {
                    inventory.TurnInActiveItems();
                    criteriaItem.turnIn();
                    Debug.Log("Turned in " + criteriaItem.getHave() + " " + criteriaItem.item.itemName);

                    if (FulfilledAllCriteria())
                    {
                        LeaveServerRpc();
                    }
                }
            }
        }
        else
        {
            TipsManager.Instance.SetTip("Incorrect Order", 2f);
        }
    }
    private bool FulfilledAllCriteria()
    {
        foreach (Criteria.Required criteriaItem in criteria.objectPairs)
        {
            if (criteriaItem.getHave() != criteriaItem.need)
            {
                return false;
            }
        }

        return true;
    }

    public void SetOrder(CraftableItem orderItem)
    {
        if (orderItem != null)
        {
            hasOrder = true;
        }
    }

    public void EnterChair(Chair chair)
    {
        currentChair = chair;
    }
}


