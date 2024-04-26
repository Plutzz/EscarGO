using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FoodProjectile : NetworkBehaviour
{
    public Rigidbody rb;
    public Player ownerOfProjectile;
    public float launchSpeed;
    public Vector3 launchDirection;
    public float remainingLifetime;
    public void Launch() {

        rb.isKinematic = false;
        //Debug.Log("Projectile initial rotation: " + transform.rotation.eulerAngles);

        Vector3 rotationAngle = transform.rotation.eulerAngles;     
        if (rotationAngle.x > 180) {                    //Aiming up
            float upwardAngle =  (rotationAngle.x - 360f);
            launchSpeed *= 1.1f;
            upwardAngle *= 0.1f;
            rotationAngle.x += upwardAngle;
        }

        rb.velocity = Quaternion.Euler(rotationAngle.x - 30, rotationAngle.y , rotationAngle.z) * launchDirection.normalized * launchSpeed;

        Destroy(gameObject, remainingLifetime);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>() != ownerOfProjectile)
        {
            other.gameObject.GetComponent<PlayerStateMachine>().Stunned();
            Destroy(gameObject);
        }
    }

    
}
