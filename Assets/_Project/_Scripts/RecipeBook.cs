using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq.Expressions;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private List<Sprite> pageImages;
    [SerializeField] private Image imageLeft;
    [SerializeField] private Image imageRight;
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] int index = -1;

    private float initialPosition = 310;

    void Start()
    {
        // Display Cover
        if (pageImages.Count > 0)
        {
            imageRight.transform.localPosition = new Vector3(0, 0, 0);
            imageRight.sprite = pageImages[0];
        }
    }
    private void OnEnable()
    {
        inventoryParent.gameObject.SetActive(false);
        if (pageImages.Count > 0)
        {
            imageRight.enabled = true;
            imageRight.sprite = pageImages[index];

            if (index == 0)
            {
                imageRight.transform.localPosition = new Vector3(0, 0, 0);
                imageLeft.enabled = false;
                imageLeft.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                imageRight.transform.localPosition = new Vector3(initialPosition, 0, 0);
                imageLeft.transform.GetChild(0).gameObject.SetActive(true);
                imageLeft.enabled = true;
                imageLeft.sprite = pageImages[index - 1];
            }
        }
    }

    private void OnDisable()
    {
        inventoryParent.gameObject.SetActive(true);
    }
    public void ChangeNextPage()
    {

        if (index < pageImages.Count - 2)
        {
            index+= 2;
            if (index < pageImages.Count - 1)
            {
                AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.PageTurn, transform.position);
                imageRight.sprite = pageImages[index];
                if (index == 0)
                {
                    imageRight.transform.localPosition = new Vector3(0, 0, 0);
                    imageLeft.enabled = false;
                    imageLeft.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    imageRight.transform.localPosition = new Vector3(initialPosition, 0, 0);
                    imageLeft.transform.GetChild(0).gameObject.SetActive(true);
                    imageLeft.enabled = true;
                    imageLeft.sprite = pageImages[index - 1];
                }
                
            }


        }

    }

    public void ChangePrevPage()
    {

        if (index > 0)
        {
            index -= 2;
            AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.PageTurn, transform.position);
            if (index == 0)
            {
                imageRight.transform.localPosition = new Vector3(0, 0, 0);
                imageLeft.enabled = false;
                imageLeft.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                imageRight.transform.localPosition = new Vector3(initialPosition, 0, 0);
                imageLeft.transform.GetChild(0).gameObject.SetActive(true);
                imageLeft.enabled = true;
                imageLeft.sprite = pageImages[index - 1];
            }
            imageRight.sprite = pageImages[index];
   
        }
    }



}
