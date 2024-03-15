using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Chair assignedChair;
    private bool inChair;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        AssignCustomerToChair();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (assignedChair != null)
        {
            Vector3 direction = assignedChair.transform.position - transform.position;
            // Move the customer until they get to the chair
            if(!inChair)
            {
                transform.position += direction.normalized * speed * Time.deltaTime;
                // If customer is close to the chair, stop moving them
                if (direction.magnitude < 0.05)
                {
                    inChair = true;
                    transform.rotation = assignedChair.transform.rotation;
                }
            }
            else
            {

            }
            
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
