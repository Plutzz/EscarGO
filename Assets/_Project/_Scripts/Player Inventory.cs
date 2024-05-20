using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Networking;
public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image inventoryBackdrop;
    [SerializeField] private Color[] inventoryColors;

    private List<InventorySpaceUI> inventorySpaceUIs = new List<InventorySpaceUI>();

    private List<InventoryItem> currentItems = new List<InventoryItem>();
    private int activeItemIndex;
    private Dictionary<string, int> itemDictionary = new Dictionary<string, int>();
    public override void OnNetworkSpawn()
    {

        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        foreach (Transform child in inventoryParent.transform) { 
            InventorySpaceUI space = child.GetComponent<InventorySpaceUI>();
            if (space != null)
            {
                inventorySpaceUIs.Add(space);
                space.AssignIcon(null);
            }
        }

        activeItemIndex = 0;
        UpdateInventory();

        Debug.Log("Owner ID: " + OwnerClientId);
        if ((int)OwnerClientId < inventoryColors.Length) { 
            inventoryBackdrop.color = inventoryColors[OwnerClientId];
        }
    }

    private void Update()
    {
        //UpdateTimeInInventory();
        if (Input.GetAxis("Mouse ScrollWheel") != 0) { 
            activeItemIndex = (inventorySpaceUIs.Count + activeItemIndex - (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"))) % inventorySpaceUIs.Count;
            UpdateInventory();
        }

        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

        for(int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                activeItemIndex = i;
                UpdateInventory();
                break;
            }
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            ClearInventory();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCurrentInventorySlot();
            UpdateInventory();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UseAllSelectedItems();
            
        }
    }

    private void UpdateTimeInInventory() {
        for(int i = 0; i < currentItems.Count; i++) {
            currentItems[i].timeInInventory += Time.deltaTime;
            if (currentItems[i].timeInInventory > currentItems[i].item.maxTimeInInventory) {
                if (currentItems[i].item.failedItem != null) {
                    SpoilItem(i);
                }
                
            }
            inventorySpaceUIs[i].SetTime(currentItems[i].timeInInventory, currentItems[i].item.maxTimeInInventory);

        }
    }

    private void SpoilItem(int itemIndex) {
        if (currentItems.Count <= itemIndex) {
            return;
        }
        Item failedItem = currentItems[itemIndex].item.failedItem;

        if (failedItem == null) {
            return;
        }
        EditDictionary(currentItems[itemIndex].item.itemName, -1);
        EditDictionary(failedItem.itemName, 1);
        currentItems[itemIndex] = new InventoryItem(failedItem);

        UpdateInventory();
        inventorySpaceUIs[itemIndex].SetTime(0, 1);
    }

    private void UpdateInventory() { 
        for (int i = 0; i < currentItems.Count; i++)
        {
            inventorySpaceUIs[i].AssignIcon(currentItems[i].item.itemSprite);
            inventorySpaceUIs[i].SetColor(currentItems[i].isSelected, activeItemIndex == i);
        }

        for (int i = currentItems.Count; i < inventorySpaceUIs.Count; i++)
        {
            inventorySpaceUIs[i].AssignIcon(null);
            inventorySpaceUIs[i].SetTime(0, 1);
            inventorySpaceUIs[i].SetColor(false, activeItemIndex == i);
        }
        if (activeItemIndex >= currentItems.Count)
        {
            itemNameText.text = "";
        }
        else { 
            itemNameText.text = currentItems[activeItemIndex].item.itemName;
        }
        
    }

    public bool TryAddItemToInventory(Item item) {
        if (item == null) {
            return false;
        }
        if (currentItems.Count == inventorySpaceUIs.Count) {
            return false;
        }
        InventoryItem inventoryItem = new InventoryItem(item);

        currentItems.Add(inventoryItem);

        EditDictionary(item.itemName, 1);

        UpdateInventory();

        return true;
    }

    /*public bool CanCraft(CraftableItem craftableItem) {

        //Check that player has all items in dictionary
        foreach (Ingredient ingredient in craftableItem.requiredIngredients) {
            if (!itemDictionary.ContainsKey(ingredient.item.itemName)) { 
                return false;
            }
            if (itemDictionary[ingredient.item.itemName] < ingredient.requiredAmount) {
                return false;
            }
        }
        Debug.Log("Can craft: " + craftableItem.itemName);
        return true;
    }*/

    /*public void Craft(CraftableItem craftableItem)
    {
        foreach (Ingredient ingredient in craftableItem.requiredIngredients)
        {
            Debug.Log($"Looking for: {ingredient.requiredAmount} x {ingredient.item.itemName}");
            int removedCount = 0;
            for (int i = currentItems.Count - 1; i >= 0; i--)
            {
                Debug.Log("I is " + i);
                if (currentItems[i].item.itemName == ingredient.item.name)
                {
                    currentItems.RemoveAt(i);
                    removedCount++;

                    if (removedCount >= ingredient.requiredAmount)
                    {
                        break;
                    }
                }
            }

            EditDictionary(ingredient.item.itemName, -1 * ingredient.requiredAmount);
        }

        TryAddItemToInventory(craftableItem);
    }*/

    

    private void EditDictionary(string key, int change) {
        if (itemDictionary.ContainsKey(key))
        {
            itemDictionary[key]+= change;
        }
        else
        {
            itemDictionary.Add(key, change);
        }
    }

    public void RemoveActiveItem() {
        if (currentItems.Count <= activeItemIndex) {
            return;
        }

        EditDictionary(currentItems[activeItemIndex].item.itemName, -1);
        currentItems.RemoveAt(activeItemIndex);
        UpdateInventory();
    }

    public void ClearInventory()
    {
        itemDictionary.Clear();
        currentItems.Clear();
        UpdateInventory();
    }

    public void TurnInActiveItems()
    {
        RemoveActiveItem();

        //TipsManager.Instance.SetTip("Turning in the " + currentItems[activeItemIndex].item.itemName, 2f);
        
    }

    public bool CurrentlyHasItem()
    {
        if (currentItems.Count <= activeItemIndex) {
            //TipsManager.Instance.SetTip("No item selected", 2f);
            return false;
        }

        if(currentItems[activeItemIndex] != null)
        {
            return true;
        }

        return false;
    }

    public Item getCurrentItem()
    {
        if(activeItemIndex >= 0 || activeItemIndex < currentItems.Count)
        {
            return currentItems[activeItemIndex].item;
        } else {
            return null;
        }
    }

    private void ToggleCurrentInventorySlot() {
        if (activeItemIndex < 0 || activeItemIndex >= currentItems.Count) {
            return;
        }

        currentItems[activeItemIndex].isSelected = !currentItems[activeItemIndex].isSelected;
    }

    public Dictionary<string, int> UseAllSelectedItems() { 
        Dictionary<string, int> selectedItems = new Dictionary<string, int>();

        for (int i = currentItems.Count - 1; i >= 0; i--) {
            if (currentItems[i].isSelected) {
                if (selectedItems.ContainsKey(currentItems[i].item.itemName))
                {
                    selectedItems[currentItems[i].item.itemName]++;
                }
                else {
                    selectedItems[currentItems[i].item.itemName] = 1;
                }


                EditDictionary(currentItems[i].item.itemName, -1);

                currentItems.RemoveAt(i);
            }
        }
        UpdateInventory();
        return selectedItems;
    }

    public int GetNumSelectedItems()
    {
        int count = 0;

        for (int i = currentItems.Count - 1; i >= 0; i--)
        {
            if (currentItems[i].isSelected)
            {
                count++;
            }
        }

        return count;
    }
}

public class InventoryItem {
    public Item item;
    public float timeInInventory;
    public bool isSelected;

    public InventoryItem(Item item) { 
        this.item = item;
        timeInInventory = 0;
        isSelected = false;
    }
}
