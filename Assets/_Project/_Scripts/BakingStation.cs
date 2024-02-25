using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingStation : SuperStation
{
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
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
    }

    private void Update() {

        //Tracing frosting
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.gameObject == leftKnob)
            {
                Debug.Log("knob");
                leftKnob.transform.Rotate(Vector3.up * 30);
                Debug.Log("rotated");
            }
        }
    }
}
