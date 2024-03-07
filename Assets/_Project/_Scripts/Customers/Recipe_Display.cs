using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour
{
    // [SerializeField] private CraftableItem[] recipes; // Array of CraftableItem items
    // [SerializeField] private GameObject customerPrefab; // Reference to the customer prefab to spawn
    // [SerializeField] private Vector3 orderOffset = new Vector3(0f, 1f, 0f);

    // private GameObject spawnedCustomer; // Reference to the spawned customer

    // // Start is called before the first frame update
    // void Start()
    // {
    //     if (spawnedCustomer == null) // Check if a customer has already been spawned
    //     {
    //         if (recipes != null && recipes.Length > 0)
    //         {
    //             // Select a random index within the bounds of the array
    //             int randomIndex = Random.Range(0, recipes.Length);

    //             // Instantiate the customer prefab
    //             spawnedCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);

    //             // Instantiate a new GameObject for the order sprite
    //             GameObject orderObject = new GameObject("OrderSprite");
    //             orderObject.transform.parent = spawnedCustomer.transform; // Set customer as parent for proper positioning

    //             // Position the order sprite above the customer
    //             orderObject.transform.localPosition = orderOffset;

    //             // Add a SpriteRenderer component to the order GameObject
    //             SpriteRenderer orderRenderer = orderObject.AddComponent<SpriteRenderer>();

    //             // Set the order sprite to the item's sprite
    //             CraftableItem orderItem = recipes[randomIndex];
    //             if (orderItem != null && orderItem.itemSprite != null)
    //             {
    //                 orderRenderer.sprite = orderItem.itemSprite;
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Order item or its sprite is not assigned!");
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Recipes array is empty. Make sure to assign recipes in the Inspector.");
    //         }
    //     }
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
