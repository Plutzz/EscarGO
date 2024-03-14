using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject customerSpawn;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject movementController;
    [SerializeField] private GameObject timerObjectPrefab; 
    [SerializeField] private float patienceTime; 
    [SerializeField] private float interactionDistance = 2f; 
    private float timer;
    private bool orderReceived;
    private bool hasOrder = false;
    private GameObject timerObject; 
    private Chair currentChair;

    void Start()
    {
        timer = patienceTime;
        orderReceived = false;
        timerObject = Instantiate(timerObjectPrefab, transform);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Leave();
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= interactionDistance && hasOrder)
        {
            orderReceived = true;
            //patienceFillObject.SetActive(true); // Show the patience fill object when order is received
        }

        UpdateTimerScale();

        if (orderReceived)
        {
            Exit();
        }

    }

    void UpdateTimerScale()
    {
        float scale = Mathf.Clamp01(1- timer / patienceTime)/85;
        float xScale = scale * 7/2;
        timerObject.transform.localScale = new Vector3(xScale, scale, 1f);
    }

    public void Leave()
    {
        Debug.Log("You take too long! I'm out");
        gameObject.SetActive(false); // Deactivate the customer GameObject
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; // Reset the currentChair reference
        }
    }

    public void Exit()
    {
        Debug.Log("Thank you!");
        gameObject.transform.position = customerSpawn.transform.position;
        gameObject.SetActive(false); 
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; // Reset the currentChair reference
        }
    }

    public void SetOrder(CraftableItem orderItem)
    {
        if (orderItem != null)
        {
            hasOrder = true;
        }
    }

    public void EnterChair(Chair chair)
    {
        currentChair = chair;
    }
}

