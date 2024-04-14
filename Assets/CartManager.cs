using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


// Spawns a set amount of carts into the scene
public class CartManager : NetworkBehaviour
{
    public int amountOfCarts = 5;
    public GameObject cartPrefab;
    public Transform spawnPoint;
    public float cartInterval = 15f;
    public float cartSpeed = 10f;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        //StartCoroutine(SpawnCarts());
    }
    void Update()
    {
        if (!IsServer) return;
        cartInterval -= Time.deltaTime;
        if (cartInterval <= 0)
        {
            StartCoroutine(SpawnCarts());
            cartInterval = 30f;
        }
    }

    IEnumerator SpawnCarts()
    {
        for (int i = 0; i < amountOfCarts; i++)
        {
            GameObject cart = Instantiate(cartPrefab, spawnPoint.position, new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f));
            Rigidbody rb = cart.GetComponentInChildren<Rigidbody>();
            rb.velocity = new Vector3(-cartSpeed, 0, 0);
            rb.angularVelocity = new Vector3(0, 1, 0);
            cart.GetComponent<NetworkObject>().Spawn(true);
            yield return new WaitForSeconds(1);
        }
    }
}
