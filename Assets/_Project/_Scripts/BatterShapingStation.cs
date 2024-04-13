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
    [SerializeField] private float cookTime = 5.0f;
    [SerializeField] private GameObject batterCircle;
    [SerializeField] private GameObject batterSpawnPoint;
    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject waffleIronJoint;

    [SerializeField] private CraftableItem batter;
    private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private bool success = false;

    
    private bool isBattering = false;
    private bool squeezing = false;
    private GameObject playerBatter;
    private float playerHoldTimer = 0f;

    private Animator waffleIronAnimation;
    private bool itemReady = false;
    private float timer = 0f;
    private Material timerMaterial;
    private float fillValue;

    public override void Activate(Item successfulItem)
    {
        timer = 0f;
        fillValue = 0;
        timerMaterial.SetFloat("_Fill_Amount", fillValue);

        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        playerHoldTimer = 0;
        isBattering = true;

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("MiniGames");
        Cursor.lockState = CursorLockMode.None;
        virtualCamera.enabled = true;
    }

    public override void GetItem()
    {
        if (itemReady)
        {
            inventory.TryAddItemToInventory(resultingItem);

            success = false;
            itemReady = false;
            fillValue = 0f;
            timerObject.SetActive(false);
            timerMaterial.SetFloat("_Fill_Amount", fillValue);
            timerMaterial.DisableKeyword("_USE_TEXTURE");

            return;
        } else if(success && !itemReady)
        {
            Debug.Log("item not ready yet");
            return;
        }
    }
    
    public override void DeActivate()
    {
        isBattering = false;
        Destroy(playerBatter);
        playerHoldTimer = 0;

        Cursor.lockState = CursorLockMode.Locked;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("Player");
        virtualCamera.enabled = false;
    }

    public override bool StationInUse
    {
        get { return isBattering; }
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
        waffleIronAnimation = waffleIronJoint.GetComponent<Animator>();
        waffleIronAnimation.SetTrigger("Open");
        timerMaterial = timerObject.GetComponent<Renderer>().material;

        // Make copy of timerMaterial
        timerMaterial = Instantiate(timerMaterial);
        timerObject.GetComponent<Renderer>().material = timerMaterial;
        timerObject.SetActive(false);
    }

    private void Update() {
        if(isBattering && !success)
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
        } else if (success && !isBattering)
        {
            fillValue = Mathf.Clamp(fillValue += Time.deltaTime/cookTime, 0f, 1f);
            timerMaterial.SetFloat("_Fill_Amount", fillValue);
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
        /*if(inventory.CanCraft(batter))
        {
            inventory.Craft(batter);
        }*/

        StartCoroutine(Cook());

        DeActivate();
    }

    private void Reset()
    {
        playerHoldTimer = 0;
        Destroy(playerBatter);
        playerBatter = Instantiate(batterCircle, batterSpawnPoint.transform.position, transform.rotation);
    }

    private IEnumerator Cook()
    {
        waffleIronAnimation.SetTrigger("Close");
        Debug.Log("cooking");
        success = true;
        timerObject.SetActive(true);
        timerMaterial.SetFloat("_Border_Thickness", 1);
        timerMaterial.SetTexture("_Texture", resultingItem.itemSprite.texture);
        timerMaterial.EnableKeyword("_USE_TEXTURE");
        yield return new WaitForSeconds(cookTime);
        timerMaterial.SetFloat("_Border_Thickness", 0.3f);
        waffleIronAnimation.SetTrigger("Open");
        Debug.Log("cooked");
        itemReady = true;
    }
}
