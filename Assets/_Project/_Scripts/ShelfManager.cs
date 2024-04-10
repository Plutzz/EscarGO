using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Manages each shelf with a random item and renders the 3d model of the item onto the shelf
public class ShelfManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<ProducingStation> shelfSpaces = new List<ProducingStation>();
    public int maxAmountOfItems = 5;
    public float lifeTime = 30f;
    int currentNumOfItems;

    private void Awake()
    {
        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            Item item = items[Random.Range(0, items.Count)];
            int amount = Random.Range(1, maxAmountOfItems);
            shelfSpaces[i].amountLeft = amount;
            shelfSpaces[i].AssignItem(item);
        }
    }

    void Update()
    {
        // Cart life time
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }

        // Check if all items are gone
        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            currentNumOfItems += shelfSpaces[i].amountLeft;
        }

        if (currentNumOfItems <= 0)
        {
            Destroy(gameObject);
        }

        currentNumOfItems = 0;
    }
}