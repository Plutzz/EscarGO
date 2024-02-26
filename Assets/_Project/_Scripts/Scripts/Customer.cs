using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Recipe order;
    [SerializeField] private float patienceTime = 60f; // Time in seconds until customer leaves
    private float timer;
    private bool orderRecieved;
    [SerializeField] private float interactionDistance = 2f;
    private bool hasOrder = false;
    [SerializeField] private GameObject movementController; // Reference to the Customer_Movement script
    private bool registered = false;

    //public RecipeManager recipeManager; // Reference to the RecipeManager

    // Start is called before the first frame update
    void Start()
    {
        timer = patienceTime;
        orderRecieved = false;

        RegisterWithMovementController();
        // if (recipeManager != null)
        // {
        //     // Assign a random recipe to this Customer
        //     recipeManager.AssignRandomRecipe(gameObject);
        // }
        // else
        // {
        //     Debug.LogError("RecipeManager reference is not set in the Customer script.");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Leave();
        }

        if (Vector3.Distance(player.transform.position, this.transform.position) <= interactionDistance && hasOrder)
        {
            orderRecieved = true;
        }

        if (orderRecieved == true)
        {
            Leave();
        }
    }

    void RegisterWithMovementController()
    {
        if(!registered && movementController != null)
        {
            movementController.GetComponent<Customer_Movement>().RegisterCustomer(gameObject);
            registered = true;
        }
    }

    public void Leave()
    {
        Debug.Log("You take too long! I'm out");
        gameObject.SetActive(false); // Use "SetActive" instead of "setActive"
    }
}

