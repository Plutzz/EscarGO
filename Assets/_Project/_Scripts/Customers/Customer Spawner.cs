using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Chair[] chairs;
    [SerializeField] private float spawnTime = 5f;
    private float timer;
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > spawnTime)
        {
            Debug.Log("Respawn Customer");
            timer = 0;
            GameObject spawnedCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);
            spawnedCustomer.GetComponent<CustomerMovement>().GetChairs(chairs);
        }
    }
}