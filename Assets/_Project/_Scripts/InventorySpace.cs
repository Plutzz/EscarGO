using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySpace : MonoBehaviour
{
    [SerializeField] private Image iconSpace;

    public void AssignIcon(Sprite sprite) {
        if (sprite == null) {
            iconSpace.gameObject.SetActive(false);
            return;
        }

        iconSpace.gameObject.SetActive(true);
        iconSpace.sprite = sprite;

    }
}
