using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingStation : SuperStation
{
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
    [SerializeField] private GameObject turnTarget;


    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    public override void Activate()
    {
        
    }

    public override void DeActivate()
    {
        
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start() {
        minigameLayer = LayerMask.GetMask("Minigame");
        GameObject leftTarget = Instantiate(turnTarget, leftKnob.transform.position, leftKnob.transform.rotation);
        GameObject middleTarget = Instantiate(turnTarget, middleKnob.transform.position, leftKnob.transform.rotation);
        GameObject rightTarget = Instantiate(turnTarget, rightKnob.transform.position, leftKnob.transform.rotation);
    }

    private void Update() {

        //Tracing frosting
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) && HitKnob())
        {
            //check
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            TurnKnob(leftKnob);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            TurnKnob(middleKnob);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            TurnKnob(rightKnob);
        }
    }

    private void TurnKnob(GameObject knob)
    {
        knob.transform.Rotate(Vector3.up * -30);
    }

    private bool HitKnob()
    {
        if(Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.CompareTag("Knob"))
            {
                TurnKnob(hit.collider.gameObject);
                return true;
            }
        }

        return false;
    }

    private void SetTemperature()
    {

    }
}
