using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    private float timer;
    //public GameObject counter;
    //public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 30){
            timer = 0;
            GameObject spawnedCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);
            // Rigidbody rb = spawnedCustomer.GetComponent<Rigidbody>();
            // if (rb != null)
            // {
            //     Vector3 direction = counter.transform.position - spawnedCustomer.transform.position;
            //     rb.velocity = direction.normalized * speed;
            // }
            // else
            // {
            //     Debug.LogError("Rigidbody component not found on the spawned customer.");
            // }
        }
    }
}

