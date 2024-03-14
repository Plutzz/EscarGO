using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private Chair[] chairs; // Reference all chairs
    [SerializeField] private float speed;
    private Chair assignedChair;

    private void Start()
    {
        AssignCustomerToChair();
    }

    private void Update()
    {
        // Stop moving when the customer arrives at the assigned chair
        if (assignedChair != null)
        {
            float distance = Vector3.Distance(transform.position, assignedChair.transform.position);
            if (distance > 0.01f) // Adjust this threshold as needed
            {
                Vector3 direction = assignedChair.transform.position - transform.position;
                transform.position += direction.normalized * speed * Time.deltaTime;
            }
            else
            {
                // Customer has arrived, stop moving
                Debug.Log("Customer arrived at chair.");
            }
        }
    }

    private void AssignCustomerToChair()
    {
        if(assignedChair == null)
        {
            foreach (Chair potentialChair in chairs)
            {
                if (!IsChairOccupied(potentialChair))
                {
                    break; // Found an unoccupied chair, stop searching
                }
            }
        }
    }

    private bool IsChairOccupied(Chair chair)
    {
        if (chair.currentCustomer != null)
        {
            return true; // Chair is occupied by another customer
        }
        else
        {
            // Assign this customer to the chair
            chair.currentCustomer = GetComponent<Customer>();
            assignedChair = chair;
            Debug.Log("Customer assigned to chair.");
            return false; // Chair is not occupied
        }
    }

    public void GetChairs(Chair[] _chairs)
    {
        chairs = _chairs;
    }
}
