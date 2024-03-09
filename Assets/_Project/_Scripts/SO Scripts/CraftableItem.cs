using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Items/CraftableItem")]
public class CraftableItem : Item
{
    public List<Ingredient> requiredIngredients = new List<Ingredient>();

    public override void CopyData(Item item)
    {
        base.CopyData(item);

        if (item is CraftableItem craftableItem)
        {
            requiredIngredients = craftableItem.requiredIngredients;
        }
        else {
            Debug.LogError("Non craftable object passed into a craftable object copy data");
        }
    }
}

[Serializable]
public class Ingredient {
    public Item item;
    public int requiredAmount;

    
}
