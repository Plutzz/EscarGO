using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class BakingStation : SuperStation
{

    [SerializeField] private CraftableItem donut;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int maxTurns = 11;
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
    [SerializeField] private GameObject turnTarget;

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


    public override void Activate()
    {
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        if (isBaking == true)
        {
            return;
        }

        if(leftTarget != null || middleTarget != null || rightTarget != null)
        {
            Destroy(leftTarget);
            Destroy(middleTarget);
            Destroy(rightTarget);
        }

        SetTargets();

        success = false;
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
    }

    private void Update() {

        if(isBaking)
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
        isBaking = false;
        if(inventory.CanCraft(donut)) //maybe other player knocks items out of inventory
        {
            inventory.Craft(donut);
        }
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
}
