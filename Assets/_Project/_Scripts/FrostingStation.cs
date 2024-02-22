using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrostingStation : SuperStation
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timeLimit = 10.0f;
    [SerializeField] private float threshHold = 10.0f;
    public bool startFrosting = false; //change to private after testing
    private float timer = 0.0f;
    private bool isTracing = false;
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;

    public override void Activate()
    {
        timer = timeLimit;
        startFrosting = true;
    }

    public override void DeActivate()
    {
        startFrosting = false;

    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start() {
        minigameLayer = LayerMask.GetMask("Minigame");
        timer = timeLimit; //remove after testing
        startFrosting = true; //remove after testing
    }

    void Update()
    {
        if(startFrosting == true)
        {
            //Time limit
            if ((Mathf.Clamp(timer, 0f, timeLimit) <= 0f))
            {
                Debug.Log("out of time");
                DeActivate();
            } else {
                timer -= Time.deltaTime;
                timerText.text = timer.ToString("F2");
            }

            //Tracing frosting
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
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
    }

    void StartTracing()
    {
        if (Physics.Raycast(ray, out RaycastHit playerStart, Mathf.Infinity, minigameLayer))
        {
            if(Vector3.Distance(playerStart.point, startPoint) < threshHold)
            {
                isTracing = true;
                Debug.Log("starting trace");
            }
        }
    }

    void ContinueTracing()
    {
        if (isTracing)
        {

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, minigameLayer))
            {
                Debug.Log("tracing");
            }
            else {
                success = false;
                Debug.Log("FAIL");
                //DeActivate(); if want to kick player out even if they dont finish
            }
        }
    }

    void StopTracing()
    {

        if (isTracing)
        {
            isTracing = false;

            if (Physics.Raycast(ray, out RaycastHit release, Mathf.Infinity, minigameLayer))
            {
                if(Vector3.Distance(release.point, endPoint) < threshHold)
                {
                    success = true;
                    Debug.Log("Success");
                    DeActivate();
                } else {
                    success = false;
                    Debug.Log("FAIL");
                    DeActivate();
                }
            }
        }
    }

    public void SetPoints(Transform start, Transform end)
    {
        startPoint = start.position;
        endPoint = end.position;
    }
}
