using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Chair assignedChair;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        AssignCustomerToChair();
    }

    private void Update()
    {
        if (!IsServer) return;

        // TODO: make customers stop moving when they arrive
        if (assignedChair != null)
        {
            Vector3 direction = assignedChair.transform.position - transform.position;
            transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }

    private void AssignCustomerToChair()
    {
        if(!IsServer) return;

        foreach (Chair potentialChair in CustomerSpawner.Instance.chairs)
        {
            if (!IsChairOccupied(potentialChair))
            {
                break;
            }
        }
    }

    private bool IsChairOccupied(Chair chair)
    {
        if (!IsServer) return true;
        Debug.Log(chair);
        if (chair.customer != null)
        {
            return true; // Chair is occupied by another customer
        }
        else
        {
            Debug.Log("Add customer to chair");
            chair.customer = GetComponent<Customer>();
            assignedChair = chair;
            return false; // Chair is not occupied
        }
    }

    public void GetChairs(Chair[] _chairs)
    {
        if (!IsServer) return;
    }
}
