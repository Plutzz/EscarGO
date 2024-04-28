using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptCheck : MonoBehaviour
{
    [SerializeField] private GameObject buttonPromptObj;
    [SerializeField] private GameObject buttonPromptDetailsObj;
    [SerializeField] private float rayLength = 5f;

    [SerializeField] private Material defaultOutline;
    [SerializeField] private Material highlightOutline;

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
