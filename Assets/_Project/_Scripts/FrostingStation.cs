using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrostingStation : SuperStation
{
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timeLimit = 5.0f;
    [SerializeField] private float threshHold = 1.0f;
    public bool isFrosting = false; //change to private after testing
    private float timer = 0.0f;
    private bool isTracing = false;
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;

    public override void Activate()
    {
        isFrosting = true;
        timer = timeLimit;
    }

    public override void DeActivate()
    {
        success = false;
        isFrosting = false;
        isTracing = false;
        timerText.text = "0";
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    public override GameObject VirtualCamera
    {
        get { return virtualCamera; }
        set { virtualCamera = value; }
    }

    private void Start() {
        minigameLayer = LayerMask.GetMask("Minigame");
        timer = timeLimit; //remove after testing
        isFrosting = true; //remove after testing
    }

    void Update()
    {
        if(isFrosting)
        {
            //Time limit
            if ((Mathf.Clamp(timer, 0.000f, timeLimit) <= 0f))
            {
                StartCoroutine(Failed());
            } else {
                timer -= Time.deltaTime;
                timerText.text = timer.ToString("F2");
            }

            //Tracing mouse
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
        if (Physics.Raycast(ray, out RaycastHit playerStart, 900f, minigameLayer))
        {
            if(Vector3.Distance(playerStart.point, startPoint) < threshHold)
            {
                isTracing = true;
                timerText.color = Color.blue;
            }
        }
    }

    void ContinueTracing()
    {
        if (isTracing)
        {
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, 900f, minigameLayer))
            {
                StartCoroutine(Failed());
            }
        }
    }

    void StopTracing()
    {

        if (isTracing)
        {
            isTracing = false;

            if (Physics.Raycast(ray, out RaycastHit release, 900f, minigameLayer))
            {
                if(Vector3.Distance(release.point, endPoint) < threshHold)
                {
                    StartCoroutine(Succeed());
                } else {
                    StartCoroutine(Failed());
                }
            }
        }
    }

    public void SetPoints(Transform start, Transform end)
    {
        startPoint = start.position;
        endPoint = end.position;
    }

    private IEnumerator Succeed()
    {
        timerText.color = Color.green;
        success = true;
        DeActivate();
        yield return new WaitForSeconds(1.0f);
        timerText.color = Color.black;
        //remove after testingvvv
        yield return new WaitForSeconds(1.0f);
        Activate();
    }

    private IEnumerator Failed()
    {
        timerText.color = Color.red;
        success = false;
        DeActivate();
        yield return new WaitForSeconds(1.0f);
        timerText.color = Color.black;
        //remove after testingvvv
        yield return new WaitForSeconds(1.0f);
        Activate();
    }
}
