using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProjectile : MonoBehaviour
{
    public Rigidbody rb;
    public float launchSpeed;
    public Vector3 launchDirection;
    public float remainingLifetime;

    public void Launch() {
        

        rb.velocity = Quaternion.Euler(0, transform.rotation.eulerAngles.y , 0) * launchDirection.normalized * launchSpeed;

        Destroy(this, remainingLifetime);
    }

    private void OnCollisionEnter(Collision other) {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStateMachine>().Stunned();
        }
    }

    
}
