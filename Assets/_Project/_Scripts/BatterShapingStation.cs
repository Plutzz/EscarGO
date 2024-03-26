using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class BatterShapingStation : SuperStation
{
    [SerializeField] private float goal = 5f;
    [SerializeField] private float goalRange = 1f;
    [SerializeField] private float goalSizeOfBatter = 0.7f;
    [SerializeField] private GameObject batterCircle;
    [SerializeField] private GameObject batterSpawnPoint;

    [SerializeField] private CraftableItem batter;
    private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private bool success = false;

    
    private bool isBattering = false;
    private bool squeezing = false;
    private GameObject playerBatter;
    [SerializeField]private float playerHoldTimer = 0f;

    public override void Activate(Item successfulItem)
    {
        isBattering = true;
        playerHoldTimer = 0;
        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        Cursor.lockState = CursorLockMode.None;
        virtualCamera.enabled = true;
    }

    public override void GetItem()
    {
        
    }
    
    public override void DeActivate()
    {
        isBattering = false;
        success = false;
        Destroy(playerBatter);
        playerHoldTimer = 0;

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
                playerBatter = Instantiate(batterCircle, batterSpawnPoint.transform.position, transform.rotation);
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
                playerBatter.transform.localScale += new Vector3(goalSizeOfBatter, 0, goalSizeOfBatter) * Time.deltaTime/goal;
                squeezing = true;
            }
        }
    }

    private void CheckBatterSize()
    {
        if((goal - goalRange) <= playerHoldTimer && playerHoldTimer <= (goal + goalRange))
        {
            Succeed();
        } else {
            Reset();
        }
    }

    private void Succeed()
    {
        success = true;
        inventory.TryAddItemToInventory(resultingItem);
        /*if(inventory.CanCraft(batter))
        {
            inventory.Craft(batter);
        }*/

        DeActivate();
    }

    private void Reset()
    {
        playerHoldTimer = 0;
        Destroy(playerBatter);
        playerBatter = Instantiate(batterCircle, batterSpawnPoint.transform.position, transform.rotation);
    }
}
