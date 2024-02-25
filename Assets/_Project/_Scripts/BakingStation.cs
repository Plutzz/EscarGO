using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingStation : SuperStation
{
    [SerializeField] private int maxTurns = 11;
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
    [SerializeField] private GameObject turnTarget;

    public bool isBaking = false; //change to private after testing
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    private GameObject leftTarget;
    private GameObject middleTarget;
    private GameObject rightTarget;
    private int turnTargetLeft;
    private int turnTargetmiddle;
    private int turnTargetright;
    private int leftTurns = 0;
    private int middleTurns = 0;
    private int righTurns = 0;
    private bool leftSuccess = false;
    private bool middleSuccess = false;
    private bool rightSuccess = false;


    public override void Activate()
    {
        leftTurns = 0;
        middleTurns = 0;
        righTurns = 0;
        SetTargets();
        isBaking = true;
    }

    public override void DeActivate()
    {
        ResetKnobs();
        Destroy(leftTarget);
        Destroy(middleTarget);
        Destroy(rightTarget);
        leftSuccess = false;
        middleSuccess = false;
        rightSuccess = false;
        success = false;
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start() {
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
        minigameLayer = LayerMask.GetMask("Minigame");

        SetTargets();
    }

    private void Update() {

        if(isBaking)
        {
            //Tracing frosting
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Input.GetKeyDown(KeyCode.Q))
            {
                TurnKnob(leftKnob);
                leftTurns += 1;
                if(leftTurns > 12)
                {
                    leftTurns = 1;
                }
            }

            if(Input.GetKeyDown(KeyCode.W))
            {
                TurnKnob(middleKnob);
                middleTurns += 1;
                if(middleTurns > 12)
                {
                    middleTurns = 1;
                }
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                TurnKnob(rightKnob);
                righTurns += 1;
                if(righTurns > 12)
                {
                    righTurns = 1;
                }
            }

            CheckKnobs();
        }
    }

    private void TurnKnob(GameObject knob)
    {
        knob.transform.Rotate(Vector3.up * -30);
    }

    private void CheckKnobs()
    {
        Renderer leftRenderer = leftTarget.GetComponentInChildren<Renderer>();
        Renderer middleRenderer = middleTarget.GetComponentInChildren<Renderer>();
        Renderer rightRenderer = rightTarget.GetComponentInChildren<Renderer>();
        if(leftTurns == turnTargetLeft)
        {
            leftRenderer.material.color = Color.green;
            leftSuccess = true;
        } else {
            leftRenderer.material.color = Color.red;
            leftSuccess = false;
        }

        if(middleTurns == turnTargetmiddle)
        {
            middleRenderer.material.color = Color.green;
            middleSuccess = true;
        } else {
            middleRenderer.material.color = Color.red;
            middleSuccess = false;
        }

        if(righTurns == turnTargetright)
        {
            rightRenderer.material.color = Color.green;
            rightSuccess = true;
        } else {
            rightRenderer.material.color = Color.red;
            rightSuccess = false;
        }

        if(leftSuccess && middleSuccess && rightSuccess)
        {
            success = true;
            StartCoroutine(Succeed());
        } else {
            success = false;
        }
    }

    private void SetTargets()
    {
        turnTargetLeft = Random.Range(0, maxTurns + 1);
        turnTargetmiddle = Random.Range(0, maxTurns + 1);
        turnTargetright = Random.Range(0, maxTurns + 1);
        
        leftTarget = Instantiate(turnTarget, leftKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetLeft));
        middleTarget = Instantiate(turnTarget, middleKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetmiddle));
        rightTarget = Instantiate(turnTarget, rightKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetright));
    }

    private IEnumerator Succeed()
    {
        Debug.Log("success");
        isBaking = false;
        yield return new WaitForSeconds(1.0f);
        DeActivate();
        Activate();
    }

    private void ResetKnobs()
    {
        leftKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        middleKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rightKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
