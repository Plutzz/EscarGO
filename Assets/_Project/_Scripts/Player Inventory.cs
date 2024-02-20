using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryParent;
    private List<InventorySpace> inventorySpaces = new List<InventorySpace>();
    private List<Item> currentItems = new List<Item>();
    private Dictionary<string, int> items = new Dictionary<string, int>();
    void Awake()
    {
        foreach (Transform child in inventoryParent.transform) { 
            InventorySpace space = child.GetComponent<InventorySpace>();
            if (space != null)
            {
                inventorySpaces.Add(space);
                space.AssignIcon(null);
            }
        }
    }

    private void UpdateInventory() { 
        for (int i = 0; i < currentItems.Count; i++)
        {
            inventorySpaces[i].AssignIcon(currentItems[i].itemSprite);
        }

        for (int i = currentItems.Count; i < inventorySpaces.Count; i++)
        {
            inventorySpaces[i].AssignIcon(null);
        }
    }

    public bool TryAddItemToInventory(Item item) {
        if (currentItems.Count == inventorySpaces.Count) {
            return false;
        }

        currentItems.Add(item);

        EditDictionary(item.itemName, 1);

        UpdateInventory();

        return true;
    }

    public bool TryCraft(CraftableItem craftableItem) {

        //Check that player has all items in dictionary
        foreach (Ingredient ingredient in craftableItem.requiredIngredients) {
            if (!items.ContainsKey(ingredient.item.itemName)) { 
                return false;
            }
            if (items[ingredient.item.itemName] < ingredient.requiredAmount) {
                return false;
            }
        }

        //If reached here then it can be crafted
        foreach (Ingredient ingredient in craftableItem.requiredIngredients)
        {
            int removedCount = 0;
            for(int i = currentItems.Count - 1; i >= 0; i++)
            {
                if (currentItems[i].itemName == ingredient.item.name) { 
                    currentItems.RemoveAt(i);
                    removedCount++;

                    if (removedCount >= ingredient.requiredAmount) {
                        break;
                    }
                }
            }

            EditDictionary(ingredient.item.itemName, ingredient.requiredAmount);
        }

        UpdateInventory();

        return true;
    }

    

    private void EditDictionary(string key, int change) {
        if (items.ContainsKey(key))
        {
            items[key]+= change;
        }
        else
        {
            items.Add(key, change);
        }
    }
    
}
