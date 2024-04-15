using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pages : NetworkBehaviour
{
    public GameObject[] pages;
    public Material nextPage;
    public Material previousPage;

    public int currentPage = 0;

    private GameObject playerInRange = null;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        currentPage = 0;

        //nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        //previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
    }

    [ClientRpc]
    public void ChangePageClientRpc(int page)
    {
        currentPage = page;
        foreach(GameObject _page in pages)
        {
            _page.SetActive(false);
        }
        pages[page].SetActive(true);
        //nextPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
        //previousPage.SetTexture("_Texture", pages[currentPage % pages.Length]);
    }

    void Update()
    {
        //if (playerInRange != null && playerInRange.GetComponent<NetworkObject>().IsOwner)
        //{
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeNextPageServerRpc(currentPage);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangePrevPageServerRpc(currentPage);
            }
        //}
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = null;
        }
    }
}
