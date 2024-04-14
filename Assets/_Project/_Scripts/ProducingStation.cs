using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProducingStation : InteractableSpace
{
    public Item producedItem;
    public int amountLeft;
    public string stationName;

    public void AssignItem(Item item)
    {
        producedItem = item;
        stationName = producedItem.itemName;

        AddModelToShelf(item, amountLeft);
    }

    public override void Interact(PlayerInventory inventory)
    {
        if (amountLeft <= 0)
        {
            TipsManager.Instance.SetTip("No more " + producedItem.itemName + " left", 2f);
        }
        else if (inventory.TryAddItemToInventory(producedItem) == true)
        {
            TipsManager.Instance.SetTip("Received a " + producedItem.itemName, 2f);
            RemoveModelFromShelfServerRpc();
        }
        else
        {
            TipsManager.Instance.SetTip("Inventory full", 2f);
        }
    }


    public void AddModelToShelf(Item item, int count)
    {
        float spacingFactorX = 0.008f;
        float spacingFactorY = 0.005f;

        // Create a new gameobject to hold the model and set it as a child of the shelf space
        // Change to 3D model later
        for (int j = 0; j < count; j++)
        {
            float randomX = Random.Range(-spacingFactorX, spacingFactorX);
            float randomY = Random.Range(-spacingFactorY, spacingFactorY);

            GameObject item_model = new GameObject(item.itemName + " Model");
            item_model.transform.SetParent(this.transform);

            item_model.AddComponent<SpriteRenderer>();
            item_model.GetComponent<SpriteRenderer>().sprite = item.itemSprite;

            item_model.transform.SetLocalPositionAndRotation(new Vector3(randomX, randomY, 0), Quaternion.Euler(90, 0, 0));
            item_model.transform.localScale = new Vector3(0.0004f, 0.0004f, 0.0004f);
        }
            
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveModelFromShelfServerRpc()
    {
        RemoveModelClientRpc();
    }

    [ClientRpc]
    public void RemoveModelClientRpc()
    {
        Debug.Log("item taken");
        if(transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }   
        amountLeft--;
    }
}
