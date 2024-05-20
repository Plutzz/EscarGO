using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FoodProjectile : NetworkBehaviour
{
    public Rigidbody rb;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem particles;


    public float launchSpeed;
    public Vector3 launchDirection;
    public float remainingLifetime;
    private ulong thrower = 9999;

    [SerializeField] private Color[] particleColors;
    [SerializeField] private Item[] possibleItems;
    public void Launch(string itemName, ulong thrower, float force) {

        SetMeshType(itemName);
        SetParticleColor((int)thrower);

        this.thrower = thrower;
        rb.isKinematic = false;
        
        Vector3 rotationAngle = transform.rotation.eulerAngles;     
        if (rotationAngle.x > 180) {                    //Aiming up
            float upwardAngle =  (rotationAngle.x - 360f);
            launchSpeed *= 1.1f;
            upwardAngle *= 0.1f;
            rotationAngle.x += upwardAngle;
        }
        launchSpeed *= Mathf.Clamp(force, 0, 1);

        rb.velocity = Quaternion.Euler(rotationAngle.x, rotationAngle.y , rotationAngle.z) * launchDirection.normalized * launchSpeed;

        Invoke("RemoveProjectile", remainingLifetime);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>().OwnerClientId != thrower)
        {
            other.gameObject.GetComponent<PlayerStateMachine>().Stunned();
            RemoveProjectile();
        }
    }


    private void RemoveProjectile() {

        meshRenderer.enabled = false;

        float particleLifeTime = particles.main.startLifetime.curveMultiplier;
        
        particles.Stop();
        Destroy(gameObject, particleLifeTime);
    }

    private void SetMeshType(string itemName) {
        foreach (Item item in possibleItems) {
            if (item.itemName == itemName) {
                if (item.itemMesh != null) {
                    meshFilter.mesh = item.itemMesh;
                    return;
                }
                break;
            }
        }

        //Set default mesh

    }

    private void SetParticleColor(int throwerID)
    {
        if (throwerID < particleColors.Length)
        {
            var main = particles.main;
            Material mat = Instantiate(particles.GetComponent<Renderer>().material);
            mat.color = particleColors[throwerID];
            particles.GetComponent<Renderer>().material = mat;
        }
    }
}
