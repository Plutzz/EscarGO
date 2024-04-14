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
    [SerializeField] private int minCuts = 5;
    [SerializeField] private int maxCuts = 10;
    [SerializeField] private GameObject cutIndicatorPrefab;
    [SerializeField] private GameObject knife;
    private LayerMask minigameLayer;
    private bool success = false;
    private bool isCutting = false;
    private Ray ray;
    private Vector3 cutIndicatorOffset;
    private GameObject cutIndicator;
    private int neededcuts = 0;
    private int cuts = 0;
    private Vector3 knifeOffset = new Vector3(0f, 0.2f, 0f);
    private Vector3 knifePosition;
    private Quaternion knifeRotation;


    public override void Activate(Item successfulItem)
    {

        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        if(isCutting == true)
        {
            return;
        }

        knifePosition = knife.transform.position;
        knifeRotation = knife.transform.rotation;

        success = false;
        
        isCutting = true;
        //cutNumber.color = Color.black;
        neededcuts = Random.Range(minCuts, maxCuts + 1);
        cuts = neededcuts - 1;

        virtualCamera.enabled = true;

        if(cuts == 0)
        {
            cuts = 1; //unlikely but just in case
        }
        //cutNumber.text = neededcuts.ToString();

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
        Debug.Log("pos" + cutIndicator.transform.position);

        alignKnife();

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().DisablePrompts();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().ClearUIItem();
        Cursor.lockState = CursorLockMode.None;
    }

    public override void GetItem()
    {
        
    }

    public override void DeActivate()
    {
        inventory = null;

        isCutting = false;
        success = false;
        //cutNumber.text = "0";

        Cursor.lockState = CursorLockMode.Locked;
        virtualCamera.enabled = false;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().EnablePrompts();

        //ResetKnife();
    }

    public override bool StationInUse
    {
        get { return isCutting; }
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
                    //cutNumber.color = Color.blue;
                    StartCoroutine(knifeCut());
                    MoveNext();
                    Mathf.Clamp(neededcuts -= 1, 0, maxCuts);
                    if(neededcuts <= 0)
                    {
                        Succeed();
                    }
                }
            }

            //cutNumber.text = neededcuts.ToString();
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
                Debug.Log("pos" + cutIndicator.transform.position);
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
        //cutNumber.color = Color.green;
        success = true;
        //inventory.Craft(chocolate);
        inventory.TryAddItemToInventory(resultingItem);
        
        if(cutIndicator != null)
        {
            Destroy(cutIndicator);
        }

        DeActivate();
    }

    private void Fail()
    {
        //cutNumber.color = Color.red;
        success = false;

        if(cutIndicator != null)
        {
            Destroy(cutIndicator);
        }

        DeActivate();
    }

    private IEnumerator knifeCut()
    {
        float elapsedTime = 0f;
        float duration = 0.1f;
        Vector3 startPosition = knife.transform.position;
        Vector3 endPosition = cutIndicator.transform.position;
        while (elapsedTime < duration)
        {
            // Calculate how far along the interpolation we are (0 to 1)
            float t = elapsedTime / duration;
            
            // Move the object using Lerp
            knife.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // Increment the time elapsed
            elapsedTime += Time.deltaTime;

            // Wait for the end of frame before continuing
            yield return null;
        }

        if(cutIndicator != null)
        {
            alignKnife();
        } else {
            ResetKnife();
        }
    }

    private void alignKnife()
    {
        knife.transform.position = cutIndicator.transform.position + knifeOffset;
        knife.transform.rotation = Quaternion.Euler(0, cutIndicator.transform.rotation.eulerAngles.y, 0);
    }

    private void ResetKnife()
    {
        knife.transform.position = knifePosition;
        knife.transform.rotation = knifeRotation;
        Debug.Log("knife " + knife.transform.position);
    }
}

public enum CutPosition
{
    Up,
    Down,
    Left,
    Right
}