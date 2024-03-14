using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Chair[] chairs;
    [SerializeField] private CraftableItem[] recipes;
    [SerializeField] private Vector3 orderOffset = new Vector3(0f, 1f, 0f); 
    [SerializeField] private float spawnTime = 5f;
    private float timer;

    void Start()
    {
        timer = spawnTime; // Start the timer at spawn time to immediately spawn a customer
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            SpawnCustomer();
            timer = spawnTime; // Reset the timer
        }
    }

    void SpawnCustomer()
    {
        if (customerPrefab == null)
        {
            Debug.LogError("Customer prefab is not assigned!");
            return;
        }

        // Select a random index within the bounds of the recipes array
        int randomIndex = Random.Range(0, recipes.Length);

        // Instantiate the customer prefab
        GameObject spawnedCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);

        // Instantiate a new GameObject for the order sprite
        GameObject orderObject = new GameObject("OrderSprite_" + spawnedCustomer.GetInstanceID());
        orderObject.transform.parent = spawnedCustomer.transform; // Set customer as parent for proper positioning

        // Position the order sprite above the customer
        orderObject.transform.localPosition = orderOffset;

        // Add a SpriteRenderer component to the order GameObject
        SpriteRenderer orderRenderer = orderObject.AddComponent<SpriteRenderer>();

        // Set the order sprite to the item's sprite
        CraftableItem orderItem = recipes[randomIndex];
        if (orderItem != null && orderItem.itemSprite != null)
        {
            orderRenderer.sprite = orderItem.itemSprite;
        }
        else
        {
            Debug.LogWarning("Order item or its sprite is not assigned!");
        }

        // Pass the array of chairs to the customer for movement
        CustomerMovement customerMovement = spawnedCustomer.GetComponent<CustomerMovement>();
        if (customerMovement != null)
        {
            customerMovement.GetChairs(chairs);
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }
    }

}
