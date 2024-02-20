using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Items/CraftableItem")]
public class CraftableItem : Item
{
    public List<Ingredient> requiredIngredients = new List<Ingredient>();
}

[Serializable]
public class Ingredient {
    public Item item;
    public int requiredAmount;
}
