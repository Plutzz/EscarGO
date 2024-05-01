using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


// Spawns a set amount of carts into the scene
public class CartManager : NetworkSingleton<CartManager>
{
    public int amountOfCarts = 5;
    public GameObject cartPrefab;
    public Transform spawnPoint;
    public float cartSpeed = 10f;
    public float cartsAlive = 0;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
    }
    void Update()
    {
        if (!IsServer) return;
        if (cartsAlive <= 0)
        {
            cartsAlive = 0;
            StartCoroutine(SpawnCarts());
        }
    }

    IEnumerator SpawnCarts()
    {
        for (int i = 0; i < amountOfCarts; i++)
        {
            cartsAlive++;
            GameObject cart = Instantiate(cartPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody rb = cart.GetComponentInChildren<Rigidbody>();
            rb.velocity = new Vector3(-cartSpeed, 0, 0);
            rb.angularVelocity = new Vector3(0, 1, 0);
            cart.GetComponent<NetworkObject>().Spawn(true);

            yield return new WaitForSeconds(1);
        }
    }
}
