using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public Recipe[] recipes; // Array of Recipe objects

    // Assign a random recipe to the specified object
    public void AssignRandomRecipe(GameObject obj)
    {
        if (recipes != null && recipes.Length > 0)
        {
            int randomIndex = Random.Range(0, recipes.Length);

            Recipe randomRecipe = recipes[randomIndex];

            Recipe_Display recipeDisplay = obj.GetComponent<Recipe_Display>();
            if (recipeDisplay != null)
            {
                recipeDisplay.recipe = randomRecipe;
            }
            else
            {
                Debug.LogWarning("Recipe_Display component not found on the object.");
            }
        }
        else
        {
            Debug.LogWarning("No recipes available.");
        }
    }
}
