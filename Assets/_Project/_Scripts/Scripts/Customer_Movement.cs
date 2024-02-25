using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    public GameObject table;    
    public GameObject counter;
    public GameObject customer;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        //customer.transform.position = Vector3.MoveTowards(customer.transform.position, counter.transform.position, speed);
    }

    // Update is called once per frame
    //if(customer.transform.position == counter.transform.position)
    //{
        void Update()
        {
            customer.transform.position = Vector3.MoveTowards(customer.transform.position, table.transform.position, speed);
        }
    //}
}
