using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Unity.Netcode;

public class CuttingStation : SuperStation
{
    public CutPosition cutPosition;
    [SerializeField] private CraftableItem chocolate;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private TextMeshProUGUI cutNumber;
    [SerializeField] private int minCuts = 5;
    [SerializeField] private int maxCuts = 10;
    [SerializeField] private GameObject cutIndicatorPrefab;
    private LayerMask minigameLayer;
    private bool success = false;
    private bool isCutting = false;
    private Ray ray;
    private Vector3 cutIndicatorOffset;
    private GameObject cutIndicator;
    private int neededcuts = 0;
    private int cuts = 0;


    public override void Activate()
    {
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        if(isCutting == true)
        {
            return;
        }

        success = false;
        
        isCutting = true;
        cutNumber.color = Color.black;
        neededcuts = Random.Range(minCuts, maxCuts + 1);
        cuts = neededcuts - 1;

        virtualCamera.enabled = true;

        if(cuts == 0)
        {
            cuts = 1; //unlikely but just in case
        }
        cutNumber.text = neededcuts.ToString();

        Vector3 offSet = new Vector3(0f, 0f, 0f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        
        switch (cutPosition)
        {
            case CutPosition.Up:
                offSet = new Vector3(0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case CutPosition.Down:
                offSet = new Vector3(-0.4f, 0.53f, 0f);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                break;
            case CutPosition.Left:
                offSet = new Vector3(0f, 0.53f, 0.4f);
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            case CutPosition.Right:
                offSet = new Vector3(0f, 0.53f, -0.4f);
                rotation = Quaternion.Euler(90f, 90f, 0f);
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }

        cutIndicator = Instantiate(cutIndicatorPrefab, transform.position + offSet, rotation);

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        Cursor.lockState = CursorLockMode.None;
    }

    public override void DeActivate()
    {
        inventory = null;

        Debug.Log("Deactivate");
        isCutting = false;
        cutNumber.text = "0";

        Cursor.lockState = CursorLockMode.Locked;
        virtualCamera.enabled = false;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
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
        minigameLayer = LayerMask.GetMask("Minigame");
    }

    private void Update() {

        if(isCutting)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {

                if(CheckHit())
                {
                    cutNumber.color = Color.blue;
                    MoveNext();
                    Mathf.Clamp(neededcuts -= 1, 0, maxCuts);
                    if(neededcuts <= 0)
                    {
                        Debug.Log("success");
                        Succeed();
                    }
                }
            }

            cutNumber.text = neededcuts.ToString();
        }
    }

    private bool CheckHit()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.gameObject == cutIndicator)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveNext()
    {
        switch (cutPosition)
        {
            case CutPosition.Up:
                cutIndicator.transform.position -= transform.forward * (0.8f/cuts); //0.8 can be changed to size of object being cut
                break;
            case CutPosition.Down:
                cutIndicator.transform.position += transform.forward * (0.8f/cuts);
                break;
            case CutPosition.Left:
                cutIndicator.transform.position += transform.right * (0.8f/cuts);
                break;
            case CutPosition.Right:
                cutIndicator.transform.position -= transform.right * (0.8f/cuts);
                break;
            default:
                Debug.Log("Invalid cutting direction");
                break;
        }
    }

    private void Succeed()
    {
        cutNumber.color = Color.green;
        success = true;
        inventory.Craft(chocolate);
        
        if(cutIndicator != null)
        {
            Destroy(cutIndicator);
        }

        DeActivate();
    }

    private void Fail()
    {
        cutNumber.color = Color.red;
        success = false;

        if(cutIndicator != null)
        {
            Destroy(cutIndicator);
        }

        DeActivate();
    }
}

public enum CutPosition
{
    Up,
    Down,
    Left,
    Right
}