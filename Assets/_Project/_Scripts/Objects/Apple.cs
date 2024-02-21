using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : InteractableItem
{

    void Awake()
    {
        itemName = "Apple";
        Debug.Log("Apple created");
    }

}

