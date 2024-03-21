using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptCheck : MonoBehaviour
{
    [SerializeField] private GameObject buttonPromptObj;
    [SerializeField] private GameObject buttonPromptDetailsObj;
    [SerializeField] private float radius = 5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckClosestItem();
    }

    void ClearUIItem()
    {
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

        if (Physics.Raycast(ray, out hit, radius))
        {
            foundButtonPrompt = true;

            Collider col = hit.collider;

            if (col.GetComponent<ButtonPromptSet>() != null)
            {
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
        }

        if (!foundButtonPrompt)
        {
            ClearUIItem();
        }
    }
}
