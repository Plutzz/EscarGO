using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    public int assignedPlayer;
    private Chair assignedChair;
    private NavMeshAgent agent;
    private bool isLeaving;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        agent = GetComponent<NavMeshAgent>();
        AssignCustomerToChair();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (assignedChair != null)
        {

            DrawPath();




            //Vector3 direction = assignedChair.transform.position - transform.position;
            //// Move the customer until they get to the chair
            //if(!inChair)
            //{
            //    transform.position += direction.normalized * speed * Time.deltaTime;
            //    // If customer is close to the chair, stop moving them
            //    if (direction.magnitude < 0.05)
            //    {
            //        inChair = true;
            //        GetComponent<Customer>().timerStarted = true;           // Start Patience Timer
            //        transform.forward = -assignedChair.transform.right;
            //    }
            //}
            //else
            //{

            //}
            
        }
    }

    private void AssignCustomerToChair()
    {
        if (!IsServer) return;

        foreach (Chair potentialChair in CustomerSpawner.Instance.chairs[assignedPlayer])
        {
            if (!IsChairOccupied(potentialChair))
            {
                agent.destination = assignedChair.transform.position;
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
    private void DrawPath()
    {
        if (agent != null && agent.hasPath)
        {
            Vector3[] corners = agent.path.corners;

            // Draw lines between each corner of the path
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.red);
            }
        }
    }

    public void SetAgentActive(bool state)
    {
        agent.enabled = state;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
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

    public void MoveToExit()
    {
        isLeaving = true;
        transform.position = assignedChair.exitPoint;
        SetAgentActive(true);
        SetDestination(CustomerSpawner.Instance.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;

        if (other.gameObject == assignedChair.gameObject && !isLeaving)
        {
            SetAgentActive(false);
            transform.position = assignedChair.transform.position - Vector3.up;
            transform.forward = -assignedChair.transform.right;

            // Start Patience Timer
            GetComponent<Customer>().timerStarted = true;
            GetComponent<Customer>().EnterChair(assignedChair);
        }

        if (other.gameObject.name == "CustomerSpawnPoint" && isLeaving)
        {
            Destroy(gameObject);
        }
    }
}
