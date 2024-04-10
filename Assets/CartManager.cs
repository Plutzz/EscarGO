using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Spawns a set amount of carts into the scene
public class CartManager : MonoBehaviour
{
    public int amountOfCarts = 5;
    public GameObject cartPrefab;
    public Transform spawnPoint;
    public float cartInterval = 15f;
    public float cartSpeed = 10f;

    void Start()
    {
        StartCoroutine(SpawnCarts());
    }
    void Update()
    {
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
            yield return new WaitForSeconds(1);
        }
    }
}
