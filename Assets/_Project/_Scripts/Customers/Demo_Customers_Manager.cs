using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;

public class Demo_Customers_Manager : MonoBehaviour
{
    [Header("Customer Spawning")]
    [SerializeField] private Demo_Customer_Controller customerPrefab;
    [SerializeField] private float timeToSpawnCustomer;
    private float timer = 0;
    [SerializeField] private Transform customerSpawn;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToSpawnCustomer && Demo_Customer_Seating.Instance.AvailableChairCount() > 0) { 
            Demo_Customer_Controller newCustomer = Instantiate(customerPrefab, customerSpawn);
            newCustomer.transform.SetParent(null);
            timer = 0;

            newCustomer.SetChairPath(Demo_Customer_Seating.Instance.GetAvailableChair());
        }

        CheckForPlayers();
    }

    private void CheckForPlayers() {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo))
            {
                Demo_Customer_Controller customer = hitInfo.collider.gameObject.GetComponent<Demo_Customer_Controller>();

                if (customer != null)
                {
                    customer.HasEaten(customerSpawn.transform.position);
                }
            }
        }
    }
}
