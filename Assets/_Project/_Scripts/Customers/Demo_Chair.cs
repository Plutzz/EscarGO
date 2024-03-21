using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Chair : MonoBehaviour
{
    public Vector3 exitPoint { get; private set; }
    [SerializeField] private Vector3 exitOffset  = new Vector3(0, 1f, -1.5f);

    //public bool occupied = false;
    private void Start()
    {
        exitPoint = transform.position + Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * exitOffset;
        Demo_Customer_Seating.Instance.ChairAvailable(this);
    }

    

}
