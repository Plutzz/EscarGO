using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pages : NetworkBehaviour
{
    public Texture[] pages;
    public Material nextPage;
    public Material previousPage;

    public int currentPage = 0;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        currentPage = 0;

        nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
    }

    [ClientRpc]
    public void ChangePageClientRpc(int page)
    {
        currentPage = page;
        nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeNextPageServerRpc(currentPage);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangePrevPageServerRpc(currentPage);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeNextPageServerRpc(int page)
    {
        currentPage = (page + 1) % pages.Length;
        ChangePageClientRpc(currentPage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePrevPageServerRpc(int page)
    {
        if (currentPage == 0)
        {
            currentPage = pages.Length;
        }
        currentPage = (page - 1 + pages.Length) % pages.Length;
        ChangePageClientRpc(currentPage);
    }
}
