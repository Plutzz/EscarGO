using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CuttingStation : SuperStation
{
    public CutPosition cutPosition;
    [SerializeField] private TextMeshProUGUI cutNumber;
    [SerializeField] private int minCuts = 5;
    [SerializeField] private int maxCuts = 10;
    [SerializeField] private GameObject cutIndicatorPrefab;
    private LayerMask minigameLayer;
    private bool success = false;
    private bool isCutting = false;
    private Ray ray;
    private Vector3 cutIndicatorOffset;
    private GameObject cutIndicator;
    private int neededcuts = 0;
    private int cuts = 0;


    public override void Activate()
    {
        isCutting = true;
        cutNumber.color = Color.black;
        neededcuts = Random.Range(minCuts, maxCuts + 1);
        cuts = neededcuts - 1;
        if(cuts == 0)
        {
            cuts = 1; //unlikely but just in case
        }
        cutNumber.text = neededcuts.ToString();

        Vector3 offSet = new Vector3(0f, 0f, 0f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        
        switch (cutPosition)
        {
            case CutPosition.Up:
                offSet = new Vector3(0f, 0.53f, 0.4f);
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            case CutPosition.Down:
                offSet = new Vector3(0f, 0.53f, -0.4f);
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            case CutPosition.Left:
                offSet = new Vector3(-0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case CutPosition.Right:
                offSet = new Vector3(0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }

        cutIndicator = Instantiate(cutIndicatorPrefab, transform.position + offSet, rotation);
    }

    public override void DeActivate()
    {
        success = false;
        isCutting = false;
        cutNumber.text = "0";

        if(cutIndicator != null)
        {
            Destroy(cutIndicator);
        }
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start()
    {
        // remove after testing
        isCutting = true;
        neededcuts = Random.Range(minCuts, maxCuts + 1);
        cuts = neededcuts - 1;
        if(cuts == 0)
        {
            cuts = 1; //unlikely but just in case
        }
        cutNumber.text = neededcuts.ToString();

        minigameLayer = LayerMask.GetMask("Minigame");
        
        //remove after testing
        Vector3 offSet = new Vector3(0f, 0f, 0f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        
        switch (cutPosition)
        {
            case CutPosition.Up:
                offSet = new Vector3(0f, 0.53f, 0.4f); //change based on station location
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            case CutPosition.Down:
                offSet = new Vector3(0f, 0.53f, -0.4f);
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            case CutPosition.Left:
                offSet = new Vector3(-0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case CutPosition.Right:
                offSet = new Vector3(0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }

        cutIndicator = Instantiate(cutIndicatorPrefab, transform.position + offSet, rotation);
    }

    private void Update() {

        if(isCutting)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                if(CheckHit())
                {
                    cutNumber.color = Color.blue;
                    MoveNext();
                    Mathf.Clamp(neededcuts -= 1, 0, maxCuts);
                    if(neededcuts <= 0)
                    {
                        StartCoroutine(Succeed());
                    }
                } else {
                    StartCoroutine(Fail());
                }
            }

            cutNumber.text = neededcuts.ToString();
        }
    }

    private bool CheckHit()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.gameObject == cutIndicator)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveNext()
    {
        switch (cutPosition)
        {
            case CutPosition.Up:
                cutIndicator.transform.position -= transform.forward * (0.8f/cuts); //0.8 can be changed to size of object being cut
                break;
            case CutPosition.Down:
                cutIndicator.transform.position += transform.forward * (0.8f/cuts);
                break;
            case CutPosition.Left:
                cutIndicator.transform.position += transform.right * (0.8f/cuts);
                break;
            case CutPosition.Right:
                cutIndicator.transform.position -= transform.right * (0.8f/cuts);
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }
    }

    private IEnumerator Succeed()
    {
        cutNumber.color = Color.green;
        success = true;
        DeActivate();
        //remove after testingvvv
        yield return new WaitForSeconds(1.0f);
        Activate();
    }

    private IEnumerator Fail()
    {
        cutNumber.color = Color.red;
        success = false;
        DeActivate();
        //remove after testingvvv
        yield return new WaitForSeconds(1.0f);
        Activate();
    }
}

public enum CutPosition
{
    Up,
    Down,
    Left,
    Right
}