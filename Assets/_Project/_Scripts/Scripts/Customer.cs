using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public Recipe order;
    public float patienceTime = 60f; // Time in seconds until customer leaves
    private float timer;
    private bool orderRecieved;
    //public RecipeManager recipeManager; // Reference to the RecipeManager

    // Start is called before the first frame update
    void Start()
    {
        timer = patienceTime;
        orderRecieved = false;
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

        if (orderRecieved == true)
        {
            Leave();
        }
    }

    public void Leave()
    {
        Debug.Log("You take too long! I'm out");
        gameObject.SetActive(false); // Use "SetActive" instead of "setActive"
    }
}

