using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySpace : MonoBehaviour
{
    [SerializeField] private Image backdrop;
    [SerializeField] private Color activeAndSelectedColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Image iconSpace;
    [SerializeField] private Image burnedFill;
    private void Awake()
    {
        AssignIcon(null);
    }

    public void SetColor(bool isSelected, bool isActive) {
        if (isSelected && isActive) 
        {
            backdrop.color = activeAndSelectedColor;
        }
        else if (isActive) 
        { 
            backdrop.color = activeColor;
        }
        else if (isSelected) 
        {
            backdrop.color = selectedColor;
        }
        else
        {
            backdrop.color = normalColor;
        }
    }
    /*public void SetActive() { 
        backdrop.color -= normalColor;
    }*/

    /*public void SetUnselected() { 
        backdrop.color = normalColor;
    }*/

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
