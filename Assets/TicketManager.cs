using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class TicketManager : NetworkBehaviour
{
    public NetworkPrefab ticketPrefab;
    // Debugging:
    [SerializeField]
    private List<Texture> tickets = new List<Texture>();


    private List<GameObject> createdTickets = new List<GameObject>();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }


    }


    void Update()
    {
        // Debug:
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     CreateTicket();
        // }

        // if (Input.GetKeyDown(KeyCode.B) && createdTickets.Count > 0)
        // {
        //     RemoveTicket(createdTickets[Random.Range(0, createdTickets.Count - 1)]);
        // }
    }

    [ClientRpc]
    public void CreateTicketClientRpc(ClientRpcParams rpcParams = default)
    {
        CreateTicket();
    }

    public void CreateTicket()
    {
        // Buffer for tickets?
        if (createdTickets.Count >= 5)
        {
            Debug.Log("Cannot add any more than 5 tickets.");
            return;
        }
        var ticket = ticketPrefab.Instantiate(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
        // Add specified texture to ticket from customer's order
        ticket.GetComponent<Renderer>().material.SetTexture("_Texture", tickets[Random.Range(0, tickets.Count - 1)]);

        ticket.transform.SetParent(this.transform);

        createdTickets.Add(ticket);

    }

    [ClientRpc]
    public void RemoveTicketClientRpc(NetworkObject ticket, ClientRpcParams rpcParams = default)
    {
        RemoveTicket(ticket);
    }

    public void RemoveTicket(NetworkObject ticket)
    {
        if (ticket == null)
        {
            Debug.Log("Ticket is null.");
            return;
        }
        if (createdTickets.Count > 0 && createdTickets.Contains(ticket))
        {
            createdTickets.Remove(ticket);
            ticket.Despawn();
        }
        else
        {
            Debug.LogError("Ticket not found in createdTickets list.");
        }
    }
}
