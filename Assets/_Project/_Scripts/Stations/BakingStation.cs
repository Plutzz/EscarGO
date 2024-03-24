using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class BakingStation : SuperStation
{

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int maxTurns = 11;
    [SerializeField] private float bakeTime = 5.0f;
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
    [SerializeField] private GameObject turnTarget;
    [SerializeField] private GameObject timerObject;

    private bool isBaking = false;
    private bool success = false;

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
    private bool itemReady = false;
    public float timer = 0f;
    private Material timerMaterial;
    private float fillValue;

    public override void Activate(Item successfulItem)
    {

        timer = 0f;
        fillValue = 0; //only need if it does not start at 0 before game starts
        timerMaterial.SetFloat("_Fill_Amount", fillValue); //only need if it does not start at 0 before game starts

        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        if(leftTarget != null || middleTarget != null || rightTarget != null)
        {
            Destroy(leftTarget);
            Destroy(middleTarget);
            Destroy(rightTarget);
        }

        SetTargets();

        leftTurns = 0;
        middleTurns = 0;
        righTurns = 0;
        isBaking = true;
        virtualCamera.enabled = true;

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().DisablePrompts();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().ClearUIItem();
        Cursor.lockState = CursorLockMode.None;
    }

    public override void GetItem()
    {
        if (itemReady)
        {
            inventory.TryAddItemToInventory(resultingItem);

            success = false;
            itemReady = false;
            fillValue = 0f;
            timerMaterial.SetFloat("_Fill_Amount", fillValue);

            return;
        } else if(success && !itemReady)
        {
            Debug.Log("item not ready yet");
            return;
        }
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
        isBaking = false;

        Cursor.lockState = CursorLockMode.Locked;
        virtualCamera.enabled = false;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().EnablePrompts();
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    public override CinemachineVirtualCamera VirtualCamera
    {
        get { return virtualCamera; }
        set { virtualCamera = value; }
    }

    private void Start() {
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
        timerMaterial = timerObject.GetComponent<Renderer>().material;
        // Make copy of timerMaterial
        timerMaterial = Instantiate(timerMaterial);
        timerObject.GetComponent<Renderer>().material = timerMaterial;
    }

    private void Update() {

        if(isBaking && !success)
        {

            if(Input.GetKeyDown(KeyCode.Q))
            {
                TurnKnob(leftKnob);
                leftTurns += 1;
                if(leftTurns > maxTurns)
                {
                    leftTurns = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.W))
            {
                TurnKnob(middleKnob);
                middleTurns += 1;
                if(middleTurns > maxTurns)
                {
                    middleTurns = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                TurnKnob(rightKnob);
                righTurns += 1;
                if(righTurns > maxTurns)
                {
                    righTurns = 0;
                }
            }

            CheckKnobs();
        } else if (success && !isBaking)
        {
            fillValue = Mathf.Clamp(fillValue += Time.deltaTime/bakeTime, 0f, 1f);
            timerMaterial.SetFloat("_Fill_Amount", fillValue);
        }
    }

    private void TurnKnob(GameObject knob)
    {
        knob.transform.Rotate(Vector3.up * -30);
    }

    private void Succeed()
    {
        leftSuccess = false;
        middleSuccess = false;
        rightSuccess = false;

        StartCoroutine(Bake());

        DeActivate();
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
            Succeed();
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

    private void ResetKnobs()
    {
        leftKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        middleKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rightKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private IEnumerator Bake()
    {
        Debug.Log("baking");
        success = true;
        yield return new WaitForSeconds(bakeTime);
        Debug.Log("baked");
        itemReady = true;
    }
}
