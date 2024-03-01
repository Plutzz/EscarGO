using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    [SerializeField] private GameObject[] chairs;
    private List<GameObject> customers = new List<GameObject>(); // Store references to all customers
    [SerializeField] private float speed;

    void Start()
    {
        RegisterCustomer(gameObject);
    }

    void Update()
    {
        MoveCustomerToAvailableChair(gameObject);
    }

    public void RegisterCustomer(GameObject customer)
    {
        customers.Add(customer);
    }

    void MoveCustomerToAvailableChair(GameObject customer)
    {
        foreach (GameObject potentialChair in chairs)
        {
            if (!IsChairOccupied(potentialChair))
            {
                // Move towards the unoccupied chair
                Vector3 direction = potentialChair.transform.position - transform.position;
                transform.position += direction.normalized * speed * Time.deltaTime;
                break;
            }
        }
    }

    bool IsChairOccupied(GameObject chair)
    {
        foreach (GameObject otherCustomer in customers)
        {
            if (otherCustomer != gameObject && otherCustomer.activeSelf &&
                Vector3.Distance(otherCustomer.transform.position, chair.transform.position) < 0.1f)
            {
                return true; // Chair is occupied by another customer
            }
        }
        return false; // Chair is not occupied
    }
}
