using System.Collections;
using System.Collections.Generic;
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
            amountLeft--;
        }
        else
        {
            TipsManager.Instance.SetTip("Inventory full", 2f);
        }
    }
}
