using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe")]
public class Recipe : ScriptableObject
{
    public Item result;

    public List<Ingredient> requiredIngredients = new List<Ingredient>();

    public bool CanCook(Dictionary<string, int> availableIngredients) {

        foreach (Ingredient ingredient in requiredIngredients) {
            if (!availableIngredients.ContainsKey(ingredient.item.name)) { 
                return false;
            }

            if (availableIngredients[ingredient.item.name] < ingredient.requiredAmount) {
                return false;
            }
        }

        return true;
    }
}
