using DG.Tweening;
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
    public float timeForShelfToDissolve = 1f;
    int currentNumOfItems;
    private Material material;
    private bool dissolving;
    private float fillValue = 1f;

    public override void OnNetworkSpawn()
    {
        GameObject childObject = transform.GetChild(0).gameObject;

        material = childObject.GetComponent<Renderer>().material;

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
        if (lifeTime <= timeForShelfToDissolve && !dissolving)
        {
            dissolving = true;
            DespawnShelfClientRpc();
        }

        // Check if all items are gone
        for (int i = 0; i < shelfSpaces.Count; i++)
        {
            currentNumOfItems += shelfSpaces[i].amountLeft;
        }

        if (currentNumOfItems <= 0 && !dissolving)
        {
            dissolving = true;
            DespawnShelfClientRpc();
        }

        currentNumOfItems = 0;
    }

    [ClientRpc]
    private void DespawnShelfClientRpc()
    {
        if(IsServer)
        {
            DOTween.To(() => material.GetFloat("_Cutoff_Amount"), x => material.SetFloat("_Cutoff_Amount", x), 0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GetComponent<NetworkObject>().Despawn(true);
            });
        }
        else
        {
            DOTween.To(() => material.GetFloat("_Cutoff_Amount"), x => material.SetFloat("_Cutoff_Amount", x), 0f, 1f).SetEase(Ease.Linear);
        }
        
    }
}