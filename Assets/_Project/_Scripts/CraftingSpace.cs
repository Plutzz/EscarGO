using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSpace : InteractableSpace
{
    public CraftableItem craftableItem;
    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.CanCraft(craftableItem) == true) { 
            TipsManager.Instance.SetTip("Made a " + craftableItem.itemName, 3f);
            // Start Minigame
        }
        else
        {
            TipsManager.Instance.SetTip("Can't make a " + craftableItem.itemName, 2f);
            //Don't start minigame
        }
    }

    
}
