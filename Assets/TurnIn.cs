using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIn : InteractableSpace
{

    [SerializeField]
    private Criteria criteria;
    [SerializeField]
    private GameObject gameManager;

    private void Start() {
        criteria.ResetHave();
    }

    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.CurrentlyHasItem()) {

            foreach(Criteria.Required criteriaItem in criteria.objectPairs)
            {
                
                if(inventory.getCurrentItem().itemName == criteriaItem.item.itemName)
                {
                    inventory.TurnInSelectedItems();
                    criteriaItem.turnIn();

                    Debug.Log("Turned in " + criteriaItem.getHave() + " " + criteriaItem.item.itemName);

                    if(FulfilledAllCriteria())
                    {
                        Cursor.lockState = CursorLockMode.None;
                        gameManager.GetComponent<ResultsUI>().GameComplete();
                    }
                }
            }
        }
        else
        {
            TipsManager.Instance.SetTip("You have nothing", 2f);
        }
    }

    private bool FulfilledAllCriteria()
    {
        foreach(Criteria.Required criteriaItem in criteria.objectPairs)
        {
            if(criteriaItem.getHave() != criteriaItem.need)
            {
                return false;
            }
        }

        return true;
    }
}
