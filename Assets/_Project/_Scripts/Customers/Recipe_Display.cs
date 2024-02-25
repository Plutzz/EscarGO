using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe_Display : MonoBehaviour
{
    public Recipe recipe;
    public Recipe[] recipes; // Array of Recipe objects

    // Start is called before the first frame update
    void Start()
    {
        if (recipes != null && recipes.Length > 0)
        {
            // Select a random index within the bounds of the array
            int randomIndex = Random.Range(0, recipes.Length);

            // Get the recipe at the random index
            Recipe randomRecipe = recipes[randomIndex];

            // Log the name of the random recipe
            Debug.Log(randomRecipe.name);
        }
        else
        {
            Debug.LogWarning("Recipes array is empty. Make sure to assign recipes in the Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

