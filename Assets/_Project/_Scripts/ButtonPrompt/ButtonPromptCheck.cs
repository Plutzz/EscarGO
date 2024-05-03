using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptCheck : MonoBehaviour
{
    [SerializeField] private GameObject buttonPromptObj;
    [SerializeField] private GameObject buttonPromptDetailsObj;
    [SerializeField] private float rayLength = 5f;

    [SerializeField] private Material defaultOutline;
    [SerializeField] private Material highlightOutline;

    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite highlightCrosshair;
    [SerializeField] private Image crosshair;

    private bool searchPrompts = true;
    private ButtonPromptSet currentBPSet;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (searchPrompts)
        {
            CheckClosestItem();
        }
    }

    public void ClearUIItem()
    {
        if (currentBPSet != null)
        {
            currentBPSet.ChangeOutline(defaultOutline);
        }

        crosshair.sprite = defaultCrosshair;

        foreach (Transform child in buttonPromptObj.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void CheckClosestItem()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        bool foundButtonPrompt = false;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            foundButtonPrompt = true;

            Collider col = hit.collider;

            if (col.GetComponent<ButtonPromptSet>() != null)
            {
                currentBPSet = col.GetComponent<ButtonPromptSet>();
                currentBPSet.ChangeOutline(highlightOutline);
                crosshair.sprite = highlightCrosshair;

                // Add button prompts
                List<ButtonPromptDetails> bpDetails = col.GetComponent<ButtonPromptSet>().buttonPromptSet;

                if (buttonPromptObj.transform.childCount > 0)
                {
                    if (bpDetails[0] != buttonPromptObj.transform.GetChild(0).GetComponent<BPDetailsUI>().bpDetails)
                    {
                        ClearUIItem();  
                    }
                }

                if (buttonPromptObj.transform.childCount != bpDetails.Count)
                {
                    foreach (ButtonPromptDetails bpDetail in bpDetails)
                    {
                        GameObject details = Instantiate(buttonPromptDetailsObj, buttonPromptObj.transform);
                        details.GetComponent<BPDetailsUI>().bpDetails = bpDetail;
                    }
                }
                
            }
            else
            {
                ClearUIItem();
                // I DONT KNOW WHERE THIS GOES // AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.ItemTrash, transform.position);
            }
        }

        if (!foundButtonPrompt)
        {
            ClearUIItem();
        }
    }

    public void DisablePrompts()
    {
        searchPrompts = false;
    }

    public void EnablePrompts()
    {
        searchPrompts = true;
    }
}
