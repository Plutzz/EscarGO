using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public Vector3 exitPoint { get; private set; }
    [SerializeField] private Vector3 exitOffset = new Vector3(0, 1f, -1.5f);
    private bool isOccupied = false; // Indicates if the chair is occupied
    public Customer currentCustomer; // Reference to the current customer

    private void Start()
    {
        exitPoint = transform.position + Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * exitOffset;
    }

    // Function to be called when a customer enters the chair trigger
    public void AssignCustomer(Customer customer)
    {
        if (!isOccupied)
        {
            currentCustomer = customer;
            isOccupied = true;
            Debug.Log("Customer assigned to chair: " + gameObject.name);
            customer.EnterChair(this);
        }
    }

    // Function to be called when the customer leaves
    public void RemoveCustomer()
    {
        currentCustomer = null;
        isOccupied = false;
        Debug.Log("Customer removed from chair: " + gameObject.name);
    }
}
