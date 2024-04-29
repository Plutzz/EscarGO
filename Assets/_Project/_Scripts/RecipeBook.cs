using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RecipeBook : NetworkBehaviour
{
    [SerializeField] private Transform pagesLeft;
    [SerializeField] private Transform pagesRight;
    [SerializeField] private List<Sprite> pageImages;
    private Image imageLeft;
    private Image imageRight;
    [SerializeField] int index = -1;

    void Start()
    {
        pagesLeft.gameObject.SetActive(false);
        imageLeft = pagesLeft.GetComponentInChildren<Image>();
        imageRight = pagesRight.GetComponentInChildren<Image>();

        if (pageImages.Count > 0)
        {
            imageRight.sprite = pageImages[0];
        }
    }
    public void ChangeNextPage()
    {

        if (index < pageImages.Count - 2)
        {
            index++;
            if (index < pageImages.Count - 1)
            {
                imageRight.sprite = pageImages[index + 1];
                imageLeft.sprite = pageImages[index];
            }
        }

        pagesLeft.gameObject.SetActive(index >= 0);
    }

    public void ChangePrevPage()
    {

        if (index > -1)
        {
            index--;
            if (index < pageImages.Count - 1)
            {
                imageRight.sprite = pageImages[index + 1];
                if (index >= 0)
                    imageLeft.sprite = pageImages[index];
                else
                    pagesLeft.gameObject.SetActive(false);
            }
        }

        pagesLeft.gameObject.SetActive(index > 0);
    }



}
