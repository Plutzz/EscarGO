using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostingStation : Interactable
{
    //[SerializeField] private float timeLimit = 10.0f;
    [SerializeField] private float threshHold = 10.0f;

    private bool isTracing = false;
    private bool success = false;
    private int minigameLayer;

    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;

    public override void Activate()
    {
        
    }

    public override bool DeActivate()
    {
        return success;
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start() {
        minigameLayer = LayerMask.GetMask("Minigame");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartTracing();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueTracing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopTracing();
        }
    }

    void StartTracing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit playerStart, Mathf.Infinity, minigameLayer))
        {
            if(Vector3.Distance(playerStart.point, startPoint) < threshHold)
            {
                isTracing = true;
            }
        }
    }

    void ContinueTracing()
    {
        if (isTracing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, minigameLayer))
            {
                //Debug.Log(hitInfo.collider.gameObject.name);
                //Debug.Log("tracing");
            } else {
                isTracing = false;
            }
        }
    }

    void StopTracing()
    {
        if (isTracing)
        {
            isTracing = false;
            Debug.Log(isTracing);

            // if(Vector3.Distance(traceStart.position, endPoint) < threshHold)
            // {
            //     DeActivate();
            // } else {
            // }
        }
    }

    public void SetPoints(Transform start, Transform end)
    {
        startPoint = start.position;
        endPoint = end.position;
    }
}
