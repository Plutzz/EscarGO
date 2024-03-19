using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Customer_Controller : MonoBehaviour
{
    private Demo_Customer_Movement movement;
    private Demo_Chair desiredChair;
    private bool eaten = false;
    private void Awake()
    {
        movement = GetComponent<Demo_Customer_Movement>();
    }

    public void SetChairPath(Demo_Chair chair) { 
        desiredChair = chair;
        movement.SetDestination(chair.exitPoint);
    }

    public void HasEaten(Vector3 exitPosition) {
        eaten = true;
        transform.position = desiredChair.exitPoint;
        movement.SetAgentActive( true);
        movement.SetDestination(exitPosition);

        Demo_Customer_Seating.Instance.ChairAvailable(desiredChair);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == desiredChair.gameObject && eaten == false) { 
            movement.SetAgentActive(false);
            transform.position = desiredChair.transform.position + Vector3.up;
            transform.rotation = desiredChair.transform.rotation;
        }

        if (other.gameObject.name == "Exit" && eaten == true) {
            Destroy(gameObject);
        }
    }


}
