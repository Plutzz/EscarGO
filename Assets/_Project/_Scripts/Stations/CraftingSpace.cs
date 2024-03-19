using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSpace : InteractableSpace
{
    public CraftableItem craftableItem;

    [Tooltip ("Place most expensive recipes at top of list")][SerializeField] private List<Recipe> possibleRecipes;

    public SuperStation station;
    public override void Interact(PlayerInventory inventory)
    {
        Dictionary<string, int> availableItems = inventory.UseAllSelectedItems();
        station.Activate(GetChosenRecipe(availableItems));
        /*if (inventory.CanCraft(craftableItem) == true) { 
            TipsManager.Instance.SetTip("Made a " + craftableItem.itemName, 3f);
            station.Activate();
        }
        else
        {
            TipsManager.Instance.SetTip("Can't make a " + craftableItem.itemName, 2f);
            //Don't start minigame
        }*/
    }

    private Item GetChosenRecipe(Dictionary<string, int> availableItems) {
        foreach (Recipe recipe in possibleRecipes) {
            if (recipe.CanCook(availableItems)) {
                return recipe.result;
            }
        }

        return null;
    
    }

    

    
}
