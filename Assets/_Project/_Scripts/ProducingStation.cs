using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducingStation : InteractableSpace
{
    public Item producedItem;
    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.TryAddItemToInventory(producedItem) == true)
        {
            TipsManager.Instance.SetTip("Received a " + producedItem.itemName, 2f);
        }
        else {
            TipsManager.Instance.SetTip("Inventory full", 2f);
        }
    }
}
