using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;


public class KneadingStation : SuperStation
{
    [SerializeField] private float goalSizeOfDough = 1f;
    [SerializeField] private GameObject squareOfDough;
    [SerializeField] private GameObject rollingPin;
    private Vector3 doughOffset = new Vector3(0, 0.52f, 0.018f);

    [SerializeField] private CraftableItem kneadedDough;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private PlayerInventory inventory;
    private bool success;

    [Header ("RollingPin")]
    [SerializeField] private Transform top;
    [SerializeField] private Transform right;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform left;
    [SerializeField] private float durationToMove = 0.5f;
    private float startTime = 0f;
    private bool isRolling = false;
    private Vector3 rollingPinDefaultPosition;
    private Quaternion rollingPinDefaultRotation;

    private GameObject playerDough;
    private bool isKneading = false;
    private int[] keySequence;
    private int wantedKey = 0;
    private bool noFirstKey = true;

    public override void Activate()
    {
        playerDough = Instantiate(squareOfDough, transform.position + doughOffset, transform.rotation);
        isKneading = true;

        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();
        virtualCamera.enabled = true;
        isKneading = true;

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
    }

    public override void DeActivate()
    {
        isKneading = false;
        noFirstKey = true;
        wantedKey = 0;
        Destroy(playerDough);

        rollingPin.transform.position = rollingPinDefaultPosition;
        rollingPin.transform.rotation = rollingPinDefaultRotation;

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
        virtualCamera.enabled = false;
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
        rollingPinDefaultPosition = rollingPin.transform.position;
        rollingPinDefaultRotation = rollingPin.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(isKneading)
        {

            if(Input.GetKeyDown(KeyCode.W))
            {
                if(noFirstKey)
                {
                    noFirstKey = false;
                    wantedKey = 2;
                }

                if(PressedSequence(2))
                {
                    playerDough.transform.localScale += new Vector3(0, 0, 0.05f);
                    rollingPin.transform.rotation = Quaternion.Euler(new Vector3(90f, 90f, 0f));
                    isRolling = true;
                }

            } else if(Input.GetKeyDown(KeyCode.A))
            {
                if(noFirstKey)
                {
                    noFirstKey = false;
                    wantedKey = 1;
                }

                if(PressedSequence(1))
                {
                    playerDough.transform.localScale += new Vector3(0.05f, 0, 0);
                    rollingPin.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
                    isRolling = true;
                }

            } else if(Input.GetKeyDown(KeyCode.S))
            {
                if(noFirstKey)
                {
                    noFirstKey = false;
                    wantedKey = 4;
                }

                if(PressedSequence(4))
                {
                    playerDough.transform.localScale += new Vector3(0, 0, 0.05f);
                    rollingPin.transform.rotation = Quaternion.Euler(new Vector3(90f, 90f, 0f));
                    isRolling = true;
                }

            } else if(Input.GetKeyDown(KeyCode.D))
            {
                if(noFirstKey)
                {
                    noFirstKey = false;
                    wantedKey = 3;
                }

                if(PressedSequence(3))
                {
                    playerDough.transform.localScale += new Vector3(0.05f, 0, 0);
                    rollingPin.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
                    isRolling = true;
                }

            }

            switch (wantedKey)
            {
                case 1:
                    MoveRollingPin(top, bottom);
                    break;
                case 2:
                    MoveRollingPin(right, left);
                    break;
                case 3:
                    MoveRollingPin(bottom, top);
                    break;
                case 4:
                    MoveRollingPin(left, right);
                    break;
                default:
                    break;
            }

            if(playerDough.transform.localScale.x >= goalSizeOfDough && playerDough.transform.localScale.z >= goalSizeOfDough)
            {
                Succeed();
            }
        }
    }

    private bool PressedSequence(int pressed)
    {
        if(pressed == wantedKey)
        {
            wantedKey += 1;

            if(wantedKey > 4)
            {
                wantedKey = 1;
            }

            return true;
        }

        return false;
    }

    private void Succeed()
    {
        success = true;
        if(inventory.CanCraft(kneadedDough))
        {
            inventory.Craft(kneadedDough);
        }

        DeActivate();
    }

    private void MoveRollingPin(Transform start, Transform end)
    {
        if(isRolling)
        {
            startTime = Time.time;
            isRolling = false;
        }

        float t = (Time.time - startTime) / durationToMove;

        // Set our position as a fraction of the distance between the markers.
        rollingPin.transform.position = Vector3.Lerp(start.position, end.position, t);
    }
}
