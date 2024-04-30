using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using FMOD.Studio;
using System;
using FMODUnity;

public class BatterShapingStation : SuperStation
{
    [SerializeField] private float goal = 5f;
    [SerializeField] private float goalRange = 1f;
    [SerializeField] private float goalSizeOfBatter = 0.7f;
    [SerializeField] private float cookTime = 5.0f;
    [SerializeField] private float timeBeforeExpire = 5.0f;
    [SerializeField] private GameObject batterCircle;
    [SerializeField] private GameObject batterSpawnPoint;
    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject waffleIronJoint;
    public GameObject batterIndicator;

    [SerializeField] private CraftableItem batter;
    private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private bool success = false;

    
    private bool isBattering = false;
    private bool squeezing = false;
    private GameObject playerBatter;
    private GameObject playerIndicator;
    private float playerHoldTimer = 0f;

    private Animator waffleIronAnimation;
    private bool itemReady = false;
    private Material timerMaterial;
    private float fillValue;
    private bool timerActive;
    private StudioEventEmitter waffleSFX;
    private StudioEventEmitter tickingSFX;
    private bool isWaffleSFXPlaying;

    public override void Activate(CraftableItem successfulItem)
    {
        if(inUse) return;

        isBattering = true;

        fillValue = 0;
        timerMaterial.SetFloat("_Fill_Amount", fillValue);

        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        playerHoldTimer = 0;

        playerIndicator = Instantiate(batterIndicator, batterSpawnPoint.transform.position, transform.rotation);

        if(IsServer)
        {
            UseStationClientRPC(true);
            StationResultClientRPC(false);
        } else {
            UseStationServerRPC(true);
            StationResultServerRPC(false);
        }

        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.InteractState);

        virtualCamera.enabled = true;
    }

    public override void GetItem()
    {
        if (itemReady)
        {
            StopAllCoroutines();

            inventory.TryAddItemToInventory(resultingItem);

            if(IsServer)
            {
                UseStationClientRPC(false);
                StationResultClientRPC(false);
            } else {
                StationResultServerRPC(false);
                UseStationServerRPC(false);
            }

            timerActive = false;
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
        PlayWaffleSfxEmitterServerRpc(0, gameObject, false);
        isWaffleSFXPlaying = false;

        isBattering = false;

        if(IsServer)
        {
            UseStationClientRPC(false);
        } else {
            UseStationServerRPC(false);
        }

        Destroy(playerBatter);
        if(playerIndicator != null)
        {
            Destroy(playerIndicator);
        }
        Debug.Log("Destroyed batter");
        playerHoldTimer = 0;
        
        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.IdleState);

        virtualCamera.enabled = false;
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
        waffleIronAnimation = waffleIronJoint.GetComponent<Animator>();
        timerMaterial = timerObject.GetComponent<Renderer>().material;

        // Make copy of timerMaterial
        timerMaterial = Instantiate(timerMaterial);
        timerObject.GetComponent<Renderer>().material = timerMaterial;
        timerObject.SetActive(false);
    }

    private void Update() {
        if(isBattering && !success)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeActivate();
                    resultingItem = null;
                }

            if(Input.GetMouseButtonDown(0))
            {
                
                if(!isWaffleSFXPlaying)
                {
                    PlayWaffleSfxEmitterServerRpc(FMODEvents.NetworkSFXName.WafflePour, gameObject, true);
                    isWaffleSFXPlaying = true;
                }
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

                if(playerHoldTimer >= (goal + goalRange))
                {
                    DeActivate();
                    resultingItem = null;
                }

            }
        } else if (timerActive)
        {
            if (!itemReady)
            {
                fillValue = Mathf.Clamp(fillValue += Time.deltaTime / cookTime, 0f, 1f);
            }
            else
            {
                fillValue = Mathf.Clamp(fillValue -= Time.deltaTime / timeBeforeExpire, 0f, 1f);
            }

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
        if(IsServer)
        {
            StationResultClientRPC(true);
        } else {
            StationResultServerRPC(true);
        }
        /*if(inventory.CanCraft(batter))
        {
            inventory.Craft(batter);
        }*/

        Destroy(playerIndicator);

        StartCoroutine(Cook());

        DeActivate();
    }

    private void Reset()
    {
        playerHoldTimer = 0;
        Destroy(playerBatter);
        //playerBatter = Instantiate(batterCircle, batterSpawnPoint.transform.position, transform.rotation);
    }

    private IEnumerator Cook()
    {
        if(IsServer)
            {
                WaffleIronAnimationClientRPC("Close");
            } else {
                WaffleIronAnimationServerRPC("Close");
            }
        timerActive = true;
        tickingSFX = AudioManager.Instance.InitializeEventEmitter(FMODEvents.NetworkSFXName.StationTicking, timerObject);
        tickingSFX.Play();
        Debug.Log("cooking");
        timerObject.SetActive(true);
        timerMaterial.SetFloat("_Border_Thickness", 1);
        timerMaterial.SetTexture("_Texture", resultingItem.itemSprite.texture);
        timerMaterial.EnableKeyword("_USE_TEXTURE");

        yield return new WaitForSeconds(cookTime);

        tickingSFX.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.CompleteOrder, transform.position);
        timerMaterial.SetFloat("_Border_Thickness", 0.3f);
        if(IsServer)
            {
                WaffleIronAnimationClientRPC("Open");
            } else {
                WaffleIronAnimationServerRPC("Open");
            }
        Debug.Log("cooked");
        itemReady = true;

        StartCoroutine(Spoil());

    }

    private IEnumerator Spoil()
    {
        yield return new WaitForSeconds(timeBeforeExpire);

        timerActive = false;
        itemReady = false;
        fillValue = 0f;
        timerObject.SetActive(false);
        timerMaterial.SetFloat("_Fill_Amount", fillValue);
        timerMaterial.DisableKeyword("_USE_TEXTURE");
        
        if(IsServer)
        {
            UseStationClientRPC(false);
            StationResultClientRPC(false);
        } else {
            UseStationServerRPC(false);
            StationResultServerRPC(false);
        }

        resultingItem = null;
    }

    //change isBattering
    [ServerRpc(RequireOwnership=false)]
    private void UseStationServerRPC(bool state)
    {
        inUse = state;
        
        UseStationClientRPC(inUse);
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

    //waffle iron joint animation
    [ServerRpc(RequireOwnership=false)]
    private void WaffleIronAnimationServerRPC(string trigger)
    {
        waffleIronAnimation.SetTrigger(trigger);

        WaffleIronAnimationClientRPC(trigger);
    }

    [ClientRpc]
    private void WaffleIronAnimationClientRPC(string trigger)
    {
        waffleIronAnimation.SetTrigger(trigger);
    }

    [ClientRpc]
    private void PlayWaffleSfxEmitterClientRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        if (play)
        {
            waffleSFX = AudioManager.Instance.InitializeEventEmitter(sound, gameObj);
            waffleSFX.Play();
        }
        else
        {
            waffleSFX?.Stop();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayWaffleSfxEmitterServerRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        PlayWaffleSfxEmitterClientRpc(sound, gameObj, play);
    }
}
