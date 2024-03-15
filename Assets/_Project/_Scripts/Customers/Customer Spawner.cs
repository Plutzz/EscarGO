using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerSpawner : NetworkSingleton<CustomerSpawner>
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] public Chair[] chairs;
    [SerializeField] public Criteria[] recipes;
    [SerializeField] private float spawnTime = 5f;
    private float timer;
    public int customerCount = 0;
    private bool isSpawning = true;

    private void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;

        if (customerCount == chairs.Length)
        {
            isSpawning = false;
        }

        if (isSpawning && timer > spawnTime)
        {
            Debug.Log("Respawn Customer");
            timer = 0;
            SpawnCustomer();
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
            customerMovement.GetChairs(chairs);
        }
        else
        {
            Debug.LogError("Customer prefab is missing CustomerMovement component!");
        }

        // Spawn Customer on the server
        spawnedCustomer.GetComponent<NetworkObject>().Spawn(true);
    }
}
