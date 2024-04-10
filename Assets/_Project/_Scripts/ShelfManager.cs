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
    int currentNumOfItems = 0;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            ProducingStation space = child.GetComponent<ProducingStation>();
            if (space != null)
            {
                shelfSpaces.Add(space);
            }
        }

        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            Item item = items[Random.Range(0, items.Count)];
            int amount = Random.Range(1, maxAmountOfItems);
            shelfSpaces[i].AssignItem(item);
            shelfSpaces[i].amountLeft = amount;
            AddModelToShelf(item, amount);
        }
    }

    public void AddModelToShelf(Item item, int count)
    {
        float spacingFactorX = 0.008f;
        float spacingFactorY = 0.005f;

        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            if (shelfSpaces[i].producedItem == item)
            {
                // Create a new gameobject to hold the model and set it as a child of the shelf space
                // Change to 3D model later
                for (int j = 0; j < count; j++)
                {
                    float randomX = Random.Range(-spacingFactorX, spacingFactorX);
                    float randomY = Random.Range(-spacingFactorY, spacingFactorY);

                    GameObject item_model = new GameObject(item.itemName + " Model");
                    item_model.transform.SetParent(shelfSpaces[i].transform);

                    item_model.AddComponent<SpriteRenderer>();
                    item_model.GetComponent<SpriteRenderer>().sprite = item.itemSprite;

                    item_model.transform.SetLocalPositionAndRotation(new Vector3(randomX, randomY, 0), Quaternion.Euler(90, 0, 0));
                    item_model.transform.localScale = new Vector3(0.0004f, 0.0004f, 0.0004f);
                }
            }
        }
    }

    public void RemoveModelFromShelf(Item item)
    {
        // To optimize, we can save the models in a dictionary with the item as the key
        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            if (shelfSpaces[i].producedItem == item)
            {
                Destroy(shelfSpaces[i].GetComponent<SpriteRenderer>());
            }
        }
    }

    void Update()
    {
        // lifeTime -= Time.deltaTime;
        // if (lifeTime <= 0)
        // {
        //     Destroy(gameObject);
        // }

        // if (currentNumOfItems <= 0)
        // {
        //     Destroy(gameObject);
        // }

        // currentNumOfItems = 0;

        // for (int i = 0; i < shelfSpaces.Count; i++)
        // {
        //     currentNumOfItems += shelfSpaces[i].amountLeft;
        // }
    }
}