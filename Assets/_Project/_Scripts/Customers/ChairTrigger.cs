using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairTrigger : MonoBehaviour
{
    private Chair chair;

    private void Start()
    {
        chair = GetComponentInParent<Chair>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        Customer customer = other.GetComponent<Customer>();
        if (customer != null)
        {
            chair.AssignCustomer(customer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        Customer customer = other.GetComponent<Customer>();
        if (customer != null)
        {
            chair.RemoveCustomer();
        }
    }
}
