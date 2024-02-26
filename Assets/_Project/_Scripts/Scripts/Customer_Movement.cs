using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    [SerializeField] private GameObject[] chairs;         // array of chair game objects
    //public GameObject counter;
    [SerializeField] private GameObject customer;
    private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private float speed;
    private GameObject currentChair;

    // Start is called before the first frame update
    // void Start()
    // {
    //     int randomIndex = Random.Range(0, chairs.Length);
    //     chair = chairs[randomIndex];
    //     //customer.transform.position = Vector3.MoveTowards(customer.transform.position, counter.transform.position, speed);
    // }

    // // Update is called once per frame
    // //if(customer.transform.position == counter.transform.position)
    // //{
    // void Update()
    // {
    //     customer.transform.position = Vector3.MoveTowards(customer.transform.position, chair.transform.position, speed);
    // }
    //}

     public void RegisterCustomer(GameObject customer)
    {
        customers.Add(customer);
        MoveCustomerToAvailableChair(customer);
    }

    void MoveCustomerToAvailableChair(GameObject customer)
    {
        foreach (GameObject potentialChair in chairs)
        {
            bool isChairOccupied = false;
            foreach (GameObject otherCustomer in customers)
            {
                if (otherCustomer != customer && otherCustomer.activeSelf && otherCustomer.transform.position == potentialChair.transform.position)
                {
                    isChairOccupied = true;
                    break;
                }
            }

            if (!isChairOccupied)
            {
                customer.transform.position = potentialChair.transform.position;
                break;
            }
        }
    }
}
