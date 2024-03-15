using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class BatterShapingStation : SuperStation
{

    [SerializeField] private CraftableItem batter;
    private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject batterCircle;
    private bool success = false;

    private bool isBattering = false;
    private bool squeezing = false;

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
                GameObject batter = Instantiate(batterCircle, transform.position + new Vector3(0, 0.52f, 0.018f), transform.rotation);
                squeezing = true;
            }
        }
    }
}
