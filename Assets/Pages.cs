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
        if (!IsOwner)
        {
            return;
        }
        currentPage = 0;

        nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
            currentPage = (currentPage + 1) % pages.Length;
            nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentPage == 0)
            {
                currentPage = pages.Length;
            }
            
            nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
            currentPage = (currentPage - 1 + pages.Length) % pages.Length;
            previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        }
    }
}
