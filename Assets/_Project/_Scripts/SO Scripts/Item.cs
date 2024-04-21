using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Items/BaseItem")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public Mesh itemMesh;
    public string itemDescription;

    public float maxTimeInInventory;
    [HideInInspector] public float currentTimeInInventory;
    public Item failedItem;

    public virtual void CopyData(Item item) { 
        this.itemName = item.itemName;
        this.itemSprite = item.itemSprite;
        this.itemDescription = item.itemDescription;
        this.itemMesh = item.itemMesh;
        this.maxTimeInInventory = item.maxTimeInInventory;
        this.failedItem = item.failedItem;
        /*if (item.failedItem != null) { 
            this.failedItem.CopyData(item.failedItem);
        }*/
    }
    
}
