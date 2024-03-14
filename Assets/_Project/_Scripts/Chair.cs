using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    private bool isOccupied = false; // Indicates if the chair is occupied
    public Customer currentCustomer; // Reference to the current customer

    // Function to be called when a customer enters the chair trigger
    public void AssignCustomer(Customer customer)
    {
        if (!isOccupied)
        {
            currentCustomer = customer;
            isOccupied = true;
            customer.EnterChair(this);
        }
    }

    // Function to be called when the customer leaves
    public void RemoveCustomer()
    {
        currentCustomer = null;
        isOccupied = false;
    }
}
