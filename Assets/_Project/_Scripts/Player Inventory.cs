using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryParent;
    private List<InventorySpace> inventorySpaces = new List<InventorySpace>();
    private List<Item> currentItems = new List<Item>();
    private int currentItemIndex;
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

        currentItemIndex = 0;
        UpdateInventory();
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) { 
            currentItemIndex = (inventorySpaces.Count + currentItemIndex - (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"))) % inventorySpaces.Count;
            UpdateInventory();
        }
    }

    private void UpdateInventory() { 
        for (int i = 0; i < currentItems.Count; i++)
        {
            inventorySpaces[i].AssignIcon(currentItems[i].itemSprite);
            inventorySpaces[i].SetUnselected();
        }

        for (int i = currentItems.Count; i < inventorySpaces.Count; i++)
        {
            inventorySpaces[i].AssignIcon(null);
            inventorySpaces[i].SetUnselected();
        }
        inventorySpaces[currentItemIndex].SetSelected();
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

        Debug.Log("Can craft: " + craftableItem.itemName);
        //If reached here then it can be crafted
        foreach (Ingredient ingredient in craftableItem.requiredIngredients)
        {
            Debug.Log($"Looking for: {ingredient.requiredAmount} x {ingredient.item.itemName}");
            int removedCount = 0;
            for(int i = currentItems.Count - 1; i >= 0; i--)
            {
                Debug.Log("I is " + i);
                if (currentItems[i].itemName == ingredient.item.name) { 
                    currentItems.RemoveAt(i);
                    removedCount++;

                    if (removedCount >= ingredient.requiredAmount) {
                        break;
                    }
                }
            }

            EditDictionary(ingredient.item.itemName, -1 * ingredient.requiredAmount);
        }

        TryAddItemToInventory(craftableItem);

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

    public void RemoveSelectedItem() {
        if (currentItems.Count <= currentItemIndex) {
            TipsManager.Instance.SetTip("No item selected", 2f);
            return;
        }

        TipsManager.Instance.SetTip("Tossing the " + currentItems[currentItemIndex].itemName, 2f);
        EditDictionary(currentItems[currentItemIndex].itemName, -1);
        currentItems.RemoveAt(currentItemIndex);
        UpdateInventory();
    }
    
}
