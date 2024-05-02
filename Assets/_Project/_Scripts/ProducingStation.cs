using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProducingStation : InteractableSpace
{
    [SerializeField] float spacingFactorX;
    [SerializeField] float spacingFactorZ;
    public Item producedItem;
    public int amountLeft;
    public string stationName;
    [SerializeField] private Material[] foodMaterials;

    public void AssignItem(Item item)
    {
        producedItem = item;
        stationName = producedItem.itemName;

        AddModelToShelf(item, amountLeft);
    }

    public override void Interact(PlayerInventory inventory)
    {
        if (amountLeft <= 0)
        {
        }
        else if (inventory.TryAddItemToInventory(producedItem) == true)
        {
            AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.ItemPickup, transform.position);
            RemoveModelFromShelfServerRpc();
        }
        else
        {
        }
    }


    public void AddModelToShelf(Item item, int count)
    {
        // Create a new gameobject to hold the model and set it as a child of the shelf space
        // Change to 3D model later
        for (int j = 0; j < count; j++)
        {
            float randomX = Random.Range(-spacingFactorX, spacingFactorX);
            float randomZ = Random.Range(-spacingFactorZ, spacingFactorZ);

            // NO PREFAB METHOD

            //GameObject item_model = new GameObject(item.itemName + " Model");
            //item_model.transform.SetParent(this.transform);

            //item_model.AddComponent<MeshFilter>().mesh = item.itemMesh;
            //item_model.AddComponent<MeshRenderer>().materials = foodMaterials;

            //item_model.transform.SetLocalPositionAndRotation(new Vector3(randomX, shelfY, randomZ), Quaternion.Euler(-90, 0, 0));
            //item_model.transform.localScale = new Vector3(15f, 15f, 15f);

            // PREFAB METHOD

            GameObject item_model = Instantiate(item.itemPrefab, transform);
            item_model.transform.SetLocalPositionAndRotation(new Vector3(randomX, item_model.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y / 2), item_model.transform.localRotation);
        }
            
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveModelFromShelfServerRpc()
    {
        RemoveModelClientRpc();
    }

    [ClientRpc]
    public void RemoveModelClientRpc()
    {
        Debug.Log("item taken");
        if(transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }   
        amountLeft--;
    }
}
