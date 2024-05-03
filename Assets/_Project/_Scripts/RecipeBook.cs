using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq.Expressions;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private List<Sprite> pageImages;
    [SerializeField] private Sprite blankPage;
    [SerializeField] private Image imageLeft;
    [SerializeField] private Image imageRight;
    [SerializeField] int index = -1;

    void Start()
    {
        // Display Cover
        if (pageImages.Count > 0)
        {
            imageRight.sprite = pageImages[0];
        }
    }
    private void OnEnable()
    {
        if (pageImages.Count > 0)
        {
            imageRight.enabled = true;
            imageRight.sprite = pageImages[0];

            if (index == 0)
            {
                imageLeft.enabled = false;
            }
            else
            {
                imageLeft.enabled = true;
                imageLeft.sprite = blankPage;
            }
        }
    }
    public void ChangeNextPage()
    {

        if (index < pageImages.Count - 2)
        {
            index++;
            if (index < pageImages.Count - 1)
            {
                imageRight.sprite = pageImages[index];
                if (index == 0)
                {
                    imageLeft.enabled = false;
                }
                else
                {
                    imageLeft.enabled = true;
                    imageLeft.sprite = blankPage;
                }
                
            }


        }

    }

    public void ChangePrevPage()
    {

        if (index > 0)
        {
            index--;
            if (index == 0)
            {
                imageLeft.enabled = false;
            }
            else
            {
                imageLeft.enabled = true;
                imageLeft.sprite = blankPage;
            }
            imageRight.sprite = pageImages[index];
   
        }
    }



}
