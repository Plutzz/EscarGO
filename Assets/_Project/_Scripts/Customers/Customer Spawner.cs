using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerSpawner : NetworkSingleton<CustomerSpawner>
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] public Criteria[] recipes;
    [SerializeField] private float spawnTime = 5f;
    private float timer = 0;
    [HideInInspector] public int customerCount = 0;
    private bool isSpawning = true;
    [SerializeField] private int numCustomerToSpawn;

    [Header("Chairs")]
    [SerializeField] public Chair[] playerOneChairs;
    [SerializeField] public Chair[] playerTwoChairs;
    [SerializeField] public Chair[] playerThreeChairs;
    [SerializeField] public Chair[] playerFourChairs;
    public Chair[][] chairs { get; private set; }

    private void Start()
    {
        chairs = new Chair[4][];
        chairs[0] = playerOneChairs;
        chairs[1] = playerTwoChairs;
        chairs[2] = playerThreeChairs;
        chairs[3] = playerFourChairs;
    }

    private void Update()
    {
        if (!IsServer) return;

        timer -= Time.deltaTime;

        if (customerCount == numCustomerToSpawn)
        {
            isSpawning = false;
        }

        if (isSpawning && timer <= 0)
        {
            Debug.Log("Respawn Customer");
            SpawnCustomer();
            timer = spawnTime; // Reset the timer
        }
    }

    public void SpawnCustomer()
    {
        if (!IsServer) return;

        if (customerPrefab == null)
        {
            Debug.LogError("Customer prefab is not assigned!");
            return;
        }

        customerCount++;

        // Instantiate the customer prefab
        GameObject spawnedCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);

        // Pass the array of chairs to the customer for movement
        CustomerMovement customerMovement = spawnedCustomer.GetComponent<CustomerMovement>();
        if (customerMovement != null)
        {
            // Assigns a random ALIVE player to this customer
            int assignedPlayer = ScoringSingleton.Instance.alivePlayers[Random.Range(0, ScoringSingleton.Instance.alivePlayers.Count)];
            Debug.Log($"assigned player {assignedPlayer} ");
            customerMovement.assignedPlayer = assignedPlayer;
            customerMovement.GetComponent<Customer>().assignedPlayer = assignedPlayer;
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }

        // Spawn Customer on the server
        spawnedCustomer.GetComponent<NetworkObject>().Spawn(true);
    }

}
