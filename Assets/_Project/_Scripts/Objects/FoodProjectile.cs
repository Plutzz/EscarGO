using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FoodProjectile : NetworkBehaviour
{
    public Rigidbody rb;
    public float launchSpeed;
    public Vector3 launchDirection;
    public float remainingLifetime;
    public void Launch() {

        rb.isKinematic = false;
        rb.velocity = Quaternion.Euler(0, transform.rotation.eulerAngles.y , 0) * launchDirection.normalized * launchSpeed;

        Destroy(this, remainingLifetime);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStateMachine>().Stunned();
        }
    }

    
}
