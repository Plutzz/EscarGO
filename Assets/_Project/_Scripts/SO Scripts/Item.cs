using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Items/BaseItem")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
    
}
