using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySpace : MonoBehaviour
{
    [SerializeField] private Image backdrop;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Image iconSpace;
    [SerializeField] private Image burnedFill;
    private void Awake()
    {
        AssignIcon(null);
    }

    public void SetSelected() { 
        backdrop.color = selectedColor;
    }

    public void SetUnselected() { 
        backdrop.color = unselectedColor;
    }

    public void AssignIcon(Sprite sprite) {
        if (sprite == null) {
            iconSpace.gameObject.SetActive(false);
            return;
        }

        iconSpace.gameObject.SetActive(true);
        iconSpace.sprite = sprite;

    }

    public void SetTime(float currentTime, float maxTime) {
        if (maxTime <= 0) {
            return;
        }

        

        burnedFill.fillAmount = currentTime/maxTime;
    }
}
