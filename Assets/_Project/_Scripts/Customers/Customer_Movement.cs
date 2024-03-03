using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    [SerializeField] private Chair[] chairs; // reference all chairs
    [SerializeField] private float speed;
    private Chair assignedChair;
    private void Start()
    {
        AssignCustomerToChair();
    }

    private void Update()
    {
        // TODO: make customers stop moving when they arrive
        if (assignedChair != null)
        {
            Vector3 direction = assignedChair.transform.position - transform.position;
            transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }

    private void AssignCustomerToChair()
    {
        foreach (Chair potentialChair in chairs)
        {
            if (!IsChairOccupied(potentialChair))
            {
                break;
            }
        }
    }

    private bool IsChairOccupied(Chair chair)
    {
        if (chair.customer != null)
        {
            return true; // Chair is occupied by another customer
        }
        else
        {
            Debug.Log("Add customer to chair");
            chair.customer = GetComponent<Customer>();
            assignedChair = chair;
            return false; // Chair is not occupied
        }
    }

    public void GetChairs(Chair[] _chairs)
    {
        chairs = _chairs;
    }
}
