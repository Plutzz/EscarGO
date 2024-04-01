using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerSpawner : NetworkSingleton<CustomerSpawner>
{
    public SpawnMethod spawnMethod;
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

    private int playerThatGetsCustomer = 0;
    private List<int> player = new List<int>();

    public override void OnNetworkSpawn()
    {
        chairs = new Chair[4][];
        chairs[0] = playerOneChairs;
        chairs[1] = playerTwoChairs;
        chairs[2] = playerThreeChairs;
        chairs[3] = playerFourChairs;

        timer = spawnTime;
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
        switch (spawnMethod)
        {
            case SpawnMethod.Random:
                SpawnRandom();
                break;
            case SpawnMethod.InOrder:
                SpawnInOrder();
                break;
            case SpawnMethod.RandomFill:
                SpawnRandomFill();
                break;
            default:
                SpawnInOrder();
                break;
        }
    }

    public void SpawnRandom()
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
            int assignedPlayer = ScoringSingleton.Instance.alivePlayers[Random.Range(0, ScoringSingleton.Instance.alivePlayers.Count)].playerNumber;
            Debug.Log($"assigned player {assignedPlayer} ");
            customerMovement.assignedPlayer = assignedPlayer;
            customerMovement.GetComponent<Customer>().assignedPlayer = assignedPlayer;
            playerThatGetsCustomer += 1;
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }

        // Spawn Customer on the server
        spawnedCustomer.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnInOrder()
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
            if(playerThatGetsCustomer > ScoringSingleton.Instance.alivePlayers.Count - 1)
            {
                playerThatGetsCustomer = 0;
            }

            int assignedPlayer = ScoringSingleton.Instance.alivePlayers[playerThatGetsCustomer].playerNumber;
            Debug.Log($"assigned player {assignedPlayer} ");
            customerMovement.assignedPlayer = assignedPlayer;
            customerMovement.GetComponent<Customer>().assignedPlayer = assignedPlayer;
            playerThatGetsCustomer += 1;
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }

        // Spawn Customer on the server
        spawnedCustomer.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnRandomFill()
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
            playerThatGetsCustomer = getRandomCustomer();

            int assignedPlayer = ScoringSingleton.Instance.alivePlayers[playerThatGetsCustomer].playerNumber;
            Debug.Log($"assigned player {assignedPlayer} ");
            customerMovement.assignedPlayer = assignedPlayer;
            customerMovement.GetComponent<Customer>().assignedPlayer = assignedPlayer;
            playerThatGetsCustomer += 1;
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }

        // Spawn Customer on the server
        spawnedCustomer.GetComponent<NetworkObject>().Spawn(true);
    }

    private void resetRandomCustomers()
    {
        player.Clear();

        for(int i = 0; i < ScoringSingleton.Instance.alivePlayers.Count; i++)
        {
            player.Add(i);
            Debug.Log("added players for customers: " + i);
        }
    }

    private int getRandomCustomer()
    {
        if(player.Count == 0 || player.Count != ScoringSingleton.Instance.alivePlayers.Count)
        {
            resetRandomCustomers();
            //Debug.Log("reset customers");
        }

        int rand = Random.Range(0, player.Count);
        //Debug.Log("rand: " + rand);

        int result = player[rand];
        player.RemoveAt(rand);

        return result;
    }

}

public enum SpawnMethod
{
    Random,
    InOrder,
    RandomFill
}
