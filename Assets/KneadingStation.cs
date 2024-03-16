using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;


public class KneadingStation : SuperStation
{
    [SerializeField] private float goalSizeOfDough = 1f;
    [SerializeField] private GameObject squareOfDough;
    private Vector3 doughOffset = new Vector3(0, 0.52f, 0.018f);

    [SerializeField] private CraftableItem kneadedDough;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private PlayerInventory inventory;
    private bool success;

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
                }

            }

            if(playerDough.transform.localScale.x >= goalSizeOfDough && playerDough.transform.localScale.z >= goalSizeOfDough)
            {
                Succeed();
            }
        }
    }

    private bool PressedSequence(int pressed)
    {
        Debug.Log("pressed = " + pressed + " | " + "wantedKey = " + wantedKey);

        if(pressed == wantedKey)
        {
            Debug.Log("correct");

            wantedKey += 1;
            if(wantedKey > 4)
            {
                wantedKey = 1;
            }

            return true;
        }

        Debug.Log("wrong");
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
}
