using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pages : NetworkBehaviour
{
    public Texture2D[] pages;
    public Shader nextPage;
    public Shader previousPage;

    public int currentPage = 0;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        currentPage = 0;
        // nextPage._Texture2D.mainTexture = pages[currentPage];
        // previousPage.GetPropertyAttributesmainTexture = pages[currentPage % pages.Length];
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // nextPage.material.mainTexture = pages[currentPage++ % pages.Length];
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentPage == 0)
            {
                currentPage = pages.Length;
            }
            // previousPage.material.mainTexture = pages[--currentPage % pages.Length];
        }
    }
}
