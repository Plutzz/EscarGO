using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float sittingOffsetX;
    [SerializeField] private float sittingOffsetY;
    [SerializeField] private float sittingOffsetZ = 0.1706f;
    public int assignedPlayer;
    private Chair assignedChair;
    private NavMeshAgent agent;
    private Customer customer;
    public bool isLeaving { get; private set; } 

    private StudioEventEmitter walkSFX;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.CustomerEnter, transform.position);
        //PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName.PlayerWalkWood, gameObject, true);
        customer = GetComponent<Customer>();
        agent = gameObject.AddComponent<NavMeshAgent>();
        SetNavMeshValues();
        AssignCustomerToChair();
    }

    private void SetNavMeshValues()
    {
        agent.radius = 0.25f;
    }

    private void Update()
    {
        if (!IsServer) return;

        if (assignedChair != null)
        {
            DrawPath();
        }
    }

    private void AssignCustomerToChair()
    {
        if (!IsServer) return;

        // Get random chair number
        int randomChair = Random.Range(0, CustomerSpawner.Instance.chairs[assignedPlayer].Length);

        for(int i = 0; i < CustomerSpawner.Instance.chairs[assignedPlayer].Length; i++)
        {
            if (!IsChairOccupied(CustomerSpawner.Instance.chairs[assignedPlayer][randomChair]))
            {
                agent.destination = assignedChair.transform.position;
                return;
            }
            
            // Keeps checking consecutive chairs until it finds an empty one

            randomChair = (randomChair + 1) % CustomerSpawner.Instance.chairs[assignedPlayer].Length;
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
            chair.currentCustomer = customer;
            assignedChair = chair;
            GetComponent<ButtonPromptSet>().meshRenderers.Add(chair.GetComponentInChildren<MeshRenderer>());
            Debug.Log("Customer assigned to chair: " + chair.gameObject.name);
            return false; // Chair is not occupied
        }
    }

    public void MoveToExit()
    {
        //PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName.PlayerWalkWood, gameObject, true);
        isLeaving = true;
        Destroy(GetComponent<ButtonPromptSet>());
        transform.position = assignedChair.exitPoint;
        SetAgentActive(true);
        SetDestination(CustomerSpawner.Instance.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;

        if (other.gameObject == assignedChair.gameObject && !isLeaving)
        {
            PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName.PlayerWalkWood, gameObject, false);

            SetAgentActive(false);
            transform.position = assignedChair.transform.position + transform.up * sittingOffsetY;

            //Vector3 localRight = transform.rotation * Vector3.forward;
            //transform.position += localRight * sittingOffsetX;

            Vector3 localForward = transform.rotation * Vector3.forward;
            transform.position += localForward * sittingOffsetZ;

            transform.forward = -assignedChair.transform.right;

            // Select a random index within the bounds of the recipes array
            // Gets the customer's order on the server for scoring
            customer.GetCustomerOrder(false);
            //int randomIndex = Random.Range(0, CustomerSpawner.Instance.recipes.Length);
            customer.GetCustomerOrderClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { ScoringSingleton.Instance.GetPlayerStats()[assignedPlayer].clientId} } }); ;


            // Start Patience Timer
            customer.ActivateTimerClientRpc(true);
            customer.EnterChair(assignedChair);


        }

        if (other.gameObject.name == "CustomerSpawnPoint" && isLeaving)
        {
            PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName.PlayerWalkWood, gameObject, false);

            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        if (play)
        {
            walkSFX = AudioManager.Instance.InitializeEventEmitter(sound, gameObj);
            walkSFX.Play();
        }
        else
        {
            walkSFX?.Stop();
        }
    }
}
