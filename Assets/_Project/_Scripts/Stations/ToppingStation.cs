using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class ToppingStation : SuperStation
{
    [SerializeField] private CraftableItem dough;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject toppingCircle;
    [SerializeField] private GameObject sprinkleParticles;
    [SerializeField] private int toppingCircleAmount = 5;
    [SerializeField] private float heightOfCircles = 0.74f;
    [SerializeField] private float maxX = 0.37f;
    [SerializeField] private float maxZ = 0.37f;
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    private bool isTopping = false;
    private int toppingCircleLeft;
    private List<GameObject> toppingCircleObjects;

    public override void Activate(Item successfulItem)
    {
        if(isTopping) return;

        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        virtualCamera.enabled = true;
        minigameLayer = LayerMask.GetMask("Minigame");

        if(IsServer)
        {
            UseStationClientRPC(true);
            StationResultClientRPC(false);
        } else {
            UseStationServerRPC(true);
            StationResultServerRPC(false);
        }

        toppingCircleLeft = toppingCircleAmount;

        for(int i = 1; i <= toppingCircleAmount; i++)
        {
            toppingCircleObjects.Add(Instantiate(toppingCircle, transform.position + new Vector3(Random.Range(-maxX, maxX), heightOfCircles, Random.Range(-maxZ, maxZ)), transform.rotation));
        }

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().DisablePrompts();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().ClearUIItem();
        Cursor.lockState = CursorLockMode.None;
    }

    public override void GetItem()
    {
        if(success)
        {
            Debug.Log("got item");
        }
    }

    public override void DeActivate()
    {
        if(IsServer)
        {
            UseStationClientRPC(false);
            StationResultClientRPC(false);
        } else {
            UseStationServerRPC(false);
            StationResultServerRPC(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        virtualCamera.enabled = false;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().EnablePrompts();

        foreach (GameObject obj in toppingCircleObjects)
        {
            Destroy(obj);
        }
    }

    public override bool StationInUse
    {
        get { return isTopping; }
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
        toppingCircleObjects = new List<GameObject>();
    }

    private void Update() {
        if(isTopping)
        {
            Debug.Log(isTopping);

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                if(HitToppingCircle())
                {
                    Vector3 screenPosition = Input.mousePosition;
                    screenPosition.z = Camera.main.nearClipPlane + 0.1f;
                    Instantiate(sprinkleParticles, Camera.main.ScreenToWorldPoint(screenPosition) , transform.rotation);
                    toppingCircleLeft -= 1;
                    
                    if(toppingCircleLeft == 0)
                    {
                        Succeed();
                    }
                }
            }

            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     DeActivate();
            // }
        }
    }

    private bool HitToppingCircle()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.CompareTag("ToppingCircle"))
            {
                Destroy(hit.collider.gameObject);
                return true;
            }
        }
        
        return false;
    }

    private void Succeed()
    {
        if(IsServer)
        {
            StationResultClientRPC(true);
        } else {
            StationResultServerRPC(true);
        }

        inventory.TryAddItemToInventory(resultingItem);
        /*if(inventory.CanCraft(dough))
        {
            inventory.Craft(dough);
        }*/
        DeActivate();
    }

    //change isTopping
    [ServerRpc(RequireOwnership=false)]
    private void UseStationServerRPC(bool state)
    {
        isTopping = state;
        
        UseStationClientRPC(isTopping);
    }

    [ClientRpc]
    private void UseStationClientRPC(bool state)
    {
        isTopping = state;
    }

    //Change station result
    [ServerRpc(RequireOwnership=false)]
    private void StationResultServerRPC(bool state)
    {
        success = state;

        StationResultClientRPC(success);
    }

    [ClientRpc]
    private void StationResultClientRPC(bool state)
    {
        success = state;
    }
}
