using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class BatterShapingStation : SuperStation
{

    [SerializeField] private float goal = 5f;
    [SerializeField] private float goalRange = 1f;

    [SerializeField] private CraftableItem batter;
    private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject batterCircle;
    private bool success = false;

    private bool isBattering = false;
    private bool squeezing = false;
    private GameObject playerBatter;
    public float playerHoldTimer = 0f;

    public override void Activate()
    {
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();
        virtualCamera.enabled = true;
        isBattering = true;

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        Cursor.lockState = CursorLockMode.None;
    }
    
    public override void DeActivate()
    {
        squeezing = false;

        Cursor.lockState = CursorLockMode.Locked;
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

    private void Update() {
        if(isBattering)
        {
            if(Input.GetMouseButtonDown(0))
            {
                playerBatter = Instantiate(batterCircle, transform.position + new Vector3(0, 0.52f, 0.018f), transform.rotation);
                squeezing = true;
            }
            
            if(Input.GetMouseButtonUp(0))
            {
                squeezing = false;
                CheckBatterSize();
            }

            if(squeezing)
            {
                playerHoldTimer += Time.deltaTime;
                playerBatter.transform.localScale += new Vector3(0.05f, 0, 0.05f) * Time.deltaTime;
                squeezing = true;
            }
        }
    }

    private void CheckBatterSize()
    {
        if((goal - goalRange) <= playerHoldTimer && playerHoldTimer <= (goal + goalRange))
        {
            Debug.Log("success");
            Succeed();
        } else {
            //fail
            Debug.Log("fail");
        }
    }

    private void Succeed()
    {
        success = true;
        if(inventory.CanCraft(batter))
        {
            inventory.Craft(batter);
        }
        DeActivate();
    }
}
