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
        // ClearUIItem();

        // Need to delete these on exit but aside from that it works
        // Checks closest item in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider col in colliders)
        {

            if (col.GetComponent<ButtonPromptSet>() != null)
            {
                List<ButtonPromptDetails> bpDetails = col.GetComponent<ButtonPromptSet>().buttonPromptSet;

                foreach (ButtonPromptDetails bpDetail in bpDetails)
                {
                    GameObject details = Instantiate(buttonPromptDetailsObj, buttonPromptObj.transform);
                    details.GetComponent<BPDetailsUI>().bpDetails = bpDetail;
                }
                
            }
            
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
