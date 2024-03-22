using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Customer : NetworkBehaviour
{
    [Header("Orders")]
    private Criteria criteria;
    [SerializeField] private GameObject orderPrefab;
    [SerializeField] private Vector3 orderOffset = new Vector3(0f, 1f, 0f);
    private Material orderMaterial;

    [Header("Timer")]
    private float timer;
    [HideInInspector] public bool timerStarted = false;
    [SerializeField] private float patienceTime = 30f; // Time in seconds until customer leaves

    [Header("Seating Variables")]
    public int assignedPlayer;
    public bool orderReceived;
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
    }

    void Update()
    {
        if (!IsServer) return;


        // Handle the timer
        if (timerStarted)
        {
            timer -= Time.deltaTime;
            UpdateTimerCircle();
        }

        if (timer <= 0 && currentChair != null)
        {
            LeaveServerRpc(false);
        }

        
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
        orderMaterial = Instantiate(orderObject.GetComponent<Renderer>().material);
        orderObject.GetComponent<Renderer>().material = orderMaterial;

        // Set the order sprite to the item's sprite
        criteria = Instantiate(CustomerSpawner.Instance.recipes[_index]);

        if (criteria != null && criteria.objectPairs[0].item.itemSprite != null)
        {
            orderMaterial.SetTexture("_Texture", criteria.objectPairs[0].item.itemSprite.texture);
        }
        else
        {
            Debug.LogWarning("Order item or its sprite is not assigned!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveServerRpc(bool gotOrder)
    {
        LeaveClientRpc();

        
        if (gotOrder)
        {
            ScoringSingleton.Instance.AddScoreServerRpc(assignedPlayer, criteria.score);
            timerStarted = false;
            StartCoroutine(FufillOrderWait(10));
        }
        else
        {
            CustomerSpawner.Instance.customerCount--;
            ScoringSingleton.Instance.RecieveStrikeServerRpc(assignedPlayer);
            Exit();
        }
    }
    private IEnumerator FufillOrderWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CustomerSpawner.Instance.customerCount--;
        Exit();
    }


    [ClientRpc]
    public void LeaveClientRpc()
    {
        DOTween.To(() => orderMaterial.GetFloat("_Radius"), x => orderMaterial.SetFloat("_Radius", x), 0, 2).SetEase(Ease.InOutExpo);
        DOTween.To(() => orderMaterial.GetFloat("_Border_Thickness"), x => orderMaterial.SetFloat("_Border_Thickness", x), 1, 1).SetEase(Ease.InOutSine);
        orderMaterial.SetFloat("_Spacing_Width", 0);
    }

    private void UpdateTimerCircle()
    {
        orderMaterial.SetFloat("_Fill_Amount", Mathf.InverseLerp(0, patienceTime, timer));
    }

    public void Exit()
    {
        Debug.Log("Thank you!");
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; 
        }
        GetComponent<CustomerMovement>().MoveToExit();
    }

    public bool TryCompleteOrder(PlayerInventory inventory)
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
                        LeaveServerRpc(true);
                    }
                }
            }
            return true;
        }
        else
        {
            TipsManager.Instance.SetTip("Incorrect Order", 2f);
            return false;
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
    public void EnterChair(Chair chair)
    {
        currentChair = chair;
    }
}


