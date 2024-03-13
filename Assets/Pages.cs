using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pages : NetworkBehaviour
{
    public Texture2D[] pages;
    public MeshRenderer nextPage;
    public MeshRenderer previousPage;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {

        }
    }
}
