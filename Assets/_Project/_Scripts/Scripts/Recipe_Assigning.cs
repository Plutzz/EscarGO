using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe_Assigning : MonoBehaviour
{
    public GameObject objectToAssignRecipeTo; // Declare the object to assign the recipe to

    void Start()
    {
        // Assuming you have a reference to the RecipeManager somewhere
        RecipeManager recipeManager = FindObjectOfType<RecipeManager>();
        
        // Check if the RecipeManager is found
        if (recipeManager != null)
        {
            // Assign a random recipe to the specified object
            recipeManager.AssignRandomRecipe(objectToAssignRecipeTo);
        }
        else
        {
            Debug.LogError("RecipeManager not found in the scene.");
        }
    }
}


