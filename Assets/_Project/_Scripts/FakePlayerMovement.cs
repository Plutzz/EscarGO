using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayerMovement : MonoBehaviour
{
    //Required components
    Rigidbody rb;

    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    //script variables
    private Vector2 inputVector;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ApplyRotation();
        ApplyVelocity();
    }

    private void GetInput() {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        inputVector = inputVector.normalized;
    }

    private void ApplyRotation() { 
        float rotationAmount = inputVector.x * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVelocity() { 
        rb.velocity = moveSpeed * inputVector.y * transform.forward;
    }
}
