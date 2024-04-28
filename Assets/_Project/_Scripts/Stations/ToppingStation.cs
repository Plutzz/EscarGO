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
    [SerializeField] private Texture2D cursorTexture;

    private bool isTopping = false;
    private int toppingCircleLeft;
    private List<GameObject> toppingCircleObjects;

    [SerializeField] private List<Item> baseItems;
    [SerializeField] private float itemOffsetY = 0.544f;
    private GameObject baseItem;


    public override void Activate(CraftableItem successfulItem)
    {
        if(inUse) return;

        isTopping = true;

        GetComponent<BoxCollider>().enabled = false;

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

        if(successfulItem.itemName != "FailedFood")
        {
            foreach(Item x in baseItems)
            {
                if(x.itemName == successfulItem.requiredIngredients[0].item.itemName)
                {
                    baseItem = Instantiate(x.itemPrefab, transform.position + new Vector3(0f, itemOffsetY, 0f), x.itemPrefab.transform.rotation);
                }
            }
        }

        for(int i = 1; i <= toppingCircleAmount; i++)
        {
            toppingCircleObjects.Add(Instantiate(toppingCircle, transform.position + new Vector3(Random.Range(-maxX, maxX), heightOfCircles, Random.Range(-maxZ, maxZ)), transform.rotation));
        }
        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.InteractState);

        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
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

        GetComponent<BoxCollider>().enabled = true;

        Destroy(baseItem);

        isTopping = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        virtualCamera.enabled = false;

        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.IdleState);

        foreach (GameObject obj in toppingCircleObjects)
        {
            Destroy(obj);
        }
    }

    public override bool StationInUse
    {
        get { return inUse; }
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
            if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeActivate();
                    resultingItem = null;
                }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                if(HitToppingCircle())
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.ToppingShake, transform.position);
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
        inUse = state;
        
        UseStationClientRPC(isTopping);
    }

    [ClientRpc]
    private void UseStationClientRPC(bool state)
    {
        inUse = state;
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
