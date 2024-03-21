using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    public int assignedPlayer;
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
                    GetComponent<Customer>().timerStarted = true;           // Start Patience Timer
                    transform.forward = -assignedChair.transform.right;
                }
            }
            else
            {

            }
            
        }
    }

    private void AssignCustomerToChair()
    {
        if (!IsServer) return;

        foreach (Chair potentialChair in CustomerSpawner.Instance.chairs[assignedPlayer])
        {
            if (!IsChairOccupied(potentialChair))
            {
                return;
            }
        }

        // If no valid chairs are found change the assigned player by adding 1 (might have some logic error)
        // pick randomly from another player?
        assignedPlayer++;

        // If all chairs are taken
        if (assignedPlayer >= NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            // Destroy this customer
            CustomerSpawner.Instance.customerCount--;
            Destroy(gameObject);
        }

    }

    private bool IsChairOccupied(Chair chair)
    {
        if (!IsServer) return true;

        Debug.Log(chair);
        if (chair.currentCustomer != null)
        {
            Debug.Log("Chair is occuppied");
            return true; // Chair is occupied by another customer
        }
        else
        {
            // Assign this customer to the chair
            chair.currentCustomer = GetComponent<Customer>();
            assignedChair = chair;
            Debug.Log("Customer assigned to chair: " + chair.gameObject.name);
            return false; // Chair is not occupied
        }
    }
}
