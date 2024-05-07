using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSpace : InteractableSpace
{
    public CraftableItem defaultItem;

    [Tooltip ("Place most expensive recipes at top of list")][SerializeField] private List<Recipe> possibleRecipes;

    public SuperStation station;
    public override void Interact(PlayerInventory inventory)
    {
        if(station.StationInUse)
        {
            return;
        } else if (station.ActivityResult)
        {
            station.GetItem();
        }
        else if (inventory.GetNumSelectedItems() <= 0) {
            return;
        }
        else
        {
            Dictionary<string, int> availableItems = inventory.UseAllSelectedItems();
            station.Activate(GetChosenRecipe(availableItems));
        }
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

    private CraftableItem GetChosenRecipe(Dictionary<string, int> availableItems) {
        foreach (Recipe recipe in possibleRecipes) {
            if (recipe.CanCook(availableItems)) {
                return recipe.result;
            }
        }
        Debug.Log("failed item got");
        return defaultItem;
    
    }

    

    
}
