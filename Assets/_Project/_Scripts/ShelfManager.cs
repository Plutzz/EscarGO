using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


// Manages each shelf with a random item and renders the 3d model of the item onto the shelf
public class ShelfManager : NetworkBehaviour
{
    public List<Item> items = new List<Item>();
    public List<ProducingStation> shelfSpaces = new List<ProducingStation>();
    public int maxAmountOfItems = 5;
    public float lifeTime = 30f;
    int currentNumOfItems;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            SetupItemClientRpc(Random.Range(0, items.Count), Random.Range(1, maxAmountOfItems), i);
        }
    }

    [ClientRpc]
    private void SetupItemClientRpc(int itemIndex, int randomAmount, int shelfNumber)
    {
        Item item = items[itemIndex];
        int amount = randomAmount;
        shelfSpaces[shelfNumber].amountLeft = amount;
        shelfSpaces[shelfNumber].AssignItem(item);
    }

    void Update()
    {
        if(!IsServer) return;
        // Cart life time
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }

        // Check if all items are gone
        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            currentNumOfItems += shelfSpaces[i].amountLeft;
        }

        if (currentNumOfItems <= 0)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }

        currentNumOfItems = 0;
    }
}