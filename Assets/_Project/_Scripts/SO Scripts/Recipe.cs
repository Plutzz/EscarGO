using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe")]
public class Recipe : ScriptableObject
{
    public CraftableItem result;

    public List<Ingredient> requiredIngredients = new List<Ingredient>();

    public bool CanCook(Dictionary<string, int> availableIngredients) {
        Debug.Log(availableIngredients.Count + " ingredients selected");
        foreach (var item in availableIngredients)
        {
            Debug.Log(item.Key + " In Inventory");
        }


        foreach (Ingredient ingredient in requiredIngredients) {
            Debug.Log(ingredient.item.name + " Required");
            if (!availableIngredients.ContainsKey(ingredient.item.name)) {
                Debug.Log("Inventory Does not contain item required");
                return false;
            }

            if (availableIngredients[ingredient.item.name] < ingredient.requiredAmount) {
                Debug.Log("Inventory Does not contain enough of item required");
                return false;
            }
        }

        return true;
    }
}
