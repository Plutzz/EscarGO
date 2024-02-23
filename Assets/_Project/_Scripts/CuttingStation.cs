using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingStation : SuperStation
{
    [SerializeField] private GameObject cutIndicator;
    private LayerMask minigameLayer;
    private bool success = false;
    private Ray ray;


    public override void Activate()
    {
        //Instantiate
    }

    public override void DeActivate()
    {
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start()
    {
        minigameLayer = LayerMask.GetMask("Minigame");
    }

    private void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if(CheckHit())
            {

            }
        }

        if (Physics.Raycast(ray, out RaycastHit playerStart, 900f, minigameLayer))
        {
            
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
    }
}