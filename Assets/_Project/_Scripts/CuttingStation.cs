using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingStation : SuperStation
{
    public CutPosition cutPosition;
    [SerializeField] private GameObject cutIndicatorPrefab;
    private LayerMask minigameLayer;
    private bool success = false;
    private Ray ray;
    private Vector3 cutIndicatorOffset;
    private GameObject cutIndicator;
    private int cuts = 0;


    public override void Activate()
    {
        //place cutposition here
        //cuts = 0;
    }

    public override void DeActivate()
    {
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
        cuts = 0; // put to activate after testing
        
        minigameLayer = LayerMask.GetMask("Minigame");
        
        //put to Activate() after testing
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

    private void Update() {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            if(CheckHit())
            {
                MoveNext();
            }
        }
    }

    private bool CheckHit()
    {
        if (Physics.Raycast(ray, out RaycastHit playerStart, 900f, minigameLayer))
        {
            return true;   
        } else {
            return false;
        }
    }

    private void MoveNext()
    {
        switch (cutPosition)
        {
            case CutPosition.Up:
                cutIndicator.transform.position -= transform.forward/20f;
                break;
            case CutPosition.Down:
                cutIndicator.transform.position += transform.forward/20f;
                break;
            case CutPosition.Left:
                cutIndicator.transform.position += transform.right/20f;
                break;
            case CutPosition.Right:
                cutIndicator.transform.position -= transform.right/20f;
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }
    }
}

public enum CutPosition
{
    Up,
    Down,
    Left,
    Right
}