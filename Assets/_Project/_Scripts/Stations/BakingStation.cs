using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.Rendering;
using FMOD.Studio;

public class BakingStation : SuperStation
{

    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int maxTurns = 11;
    [SerializeField] private float bakeTime = 5.0f;
    [SerializeField] private float timeBeforeExpire = 5.0f;
    [SerializeField] private GameObject leftKnob;
    [SerializeField] private GameObject middleKnob;
    [SerializeField] private GameObject rightKnob;
    [SerializeField] private GameObject turnTarget;
    [SerializeField] private GameObject timerObject;

    private bool isBaking = false;
    private bool success = false;

    private GameObject leftTarget;
    private GameObject middleTarget;
    private GameObject rightTarget;
    private int turnTargetLeft;
    private int turnTargetmiddle;
    private int turnTargetright;
    private int leftTurns = 0;
    private int middleTurns = 0;
    private int righTurns = 0;
    private bool leftSuccess = false;
    private bool middleSuccess = false;
    private bool rightSuccess = false;
    private bool itemReady = false;

    private bool timerActive;
    private Material timerMaterial;
    private float fillValue;
    private EventInstance ovenSFX;

    public override void Activate(CraftableItem successfulItem)
    {
        if(inUse) return;

        ResetKnobs();
        resultingItem = successfulItem;
        inventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();

        if(IsServer)
        {
            UseStationClientRPC(true);
            StationResultClientRPC(false);
        } else {
            UseStationServerRPC(true);
            StationResultServerRPC(false);
        }

        isBaking = true;
        
        fillValue = 0; //only need if it does not start at 0 before game starts
        timerMaterial.SetFloat("_Fill_Amount", fillValue); //only need if it does not start at 0 before game starts

        if(leftTarget != null || middleTarget != null || rightTarget != null)
        {
            Destroy(leftTarget);
            Destroy(middleTarget);
            Destroy(rightTarget);
        }

        SetTargets();

        leftTurns = 0;
        middleTurns = 0;
        righTurns = 0;

        virtualCamera.enabled = true;

        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.InteractState);
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
        ResetKnobs();
        Destroy(leftTarget);
        Destroy(middleTarget);
        Destroy(rightTarget);
        leftSuccess = false;
        middleSuccess = false;
        rightSuccess = false;

        isBaking = false;

        if(IsServer)
        {
            UseStationClientRPC(false);
        } else {
            UseStationServerRPC(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        virtualCamera.enabled = false;
        PlayerStateMachine stateMachine = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.IdleState);
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
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
        timerMaterial = timerObject.GetComponent<Renderer>().material;

        // Make copy of timerMaterial
        timerMaterial = Instantiate(timerMaterial);
        timerObject.GetComponent<Renderer>().material = timerMaterial;
        timerObject.SetActive(false);
    }

    private void Update() {

        if(isBaking && !success)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeActivate();
                    resultingItem = null;
                }
                
            if(Input.GetKeyDown(KeyCode.Q))
            {
                TurnKnob(leftKnob);
                leftTurns += 1;
                if(leftTurns > maxTurns)
                {
                    leftTurns = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.W))
            {
                TurnKnob(middleKnob);
                middleTurns += 1;
                if(middleTurns > maxTurns)
                {
                    middleTurns = 0;
                }
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                TurnKnob(rightKnob);
                righTurns += 1;
                if(righTurns > maxTurns)
                {
                    righTurns = 0;
                }
            }

            CheckKnobs();
        } else if (timerActive)
        {
            if(!itemReady)
            {
                fillValue = Mathf.Clamp(fillValue += Time.deltaTime / bakeTime, 0f, 1f);
            }
            else
            {
                fillValue = Mathf.Clamp(fillValue -= Time.deltaTime / timeBeforeExpire, 0f, 1f);
            }
           
            timerMaterial.SetFloat("_Fill_Amount", fillValue);
        }
    }

    private void TurnKnob(GameObject knob)
    {
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.TurnKnob, transform.position);
        knob.transform.Rotate(Vector3.up * -30);
    }

    private void Succeed()
    {
        if(IsServer)
        {
            StationResultClientRPC(true);
        } else {
            StationResultServerRPC(true);
        }
        leftSuccess = false;
        middleSuccess = false;
        rightSuccess = false;
        StartCoroutine(Bake());
        DeActivate();
    }

    private void CheckKnobs()
    {
        Renderer leftRenderer = leftTarget.GetComponentInChildren<Renderer>();
        Renderer middleRenderer = middleTarget.GetComponentInChildren<Renderer>();
        Renderer rightRenderer = rightTarget.GetComponentInChildren<Renderer>();

        if(leftTurns == turnTargetLeft)
        {
            leftRenderer.material.color = Color.green;
            leftSuccess = true;
        } else {
            leftRenderer.material.color = Color.red;
            leftSuccess = false;
        }

        if(middleTurns == turnTargetmiddle)
        {
            middleRenderer.material.color = Color.green;
            middleSuccess = true;
        } else {
            middleRenderer.material.color = Color.red;
            middleSuccess = false;
        }

        if(righTurns == turnTargetright)
        {
            rightRenderer.material.color = Color.green;
            rightSuccess = true;
        } else {
            rightRenderer.material.color = Color.red;
            rightSuccess = false;
        }

        if(leftSuccess && middleSuccess && rightSuccess)
        {
            Succeed();
        } else {
            if(IsServer)
            {
                StationResultClientRPC(false);
            } else {
                StationResultServerRPC(false);
            }
        }
    }

    private void SetTargets()
    {
        turnTargetLeft = Random.Range(0, maxTurns + 1);
        turnTargetmiddle = Random.Range(0, maxTurns + 1);
        turnTargetright = Random.Range(0, maxTurns + 1);
        
        leftTarget = Instantiate(turnTarget, leftKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetLeft));
        middleTarget = Instantiate(turnTarget, middleKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetmiddle));
        rightTarget = Instantiate(turnTarget, rightKnob.transform.position, Quaternion.Euler(new Vector3(0f, -30f, 0f) * turnTargetright));
    }

    private void ResetKnobs()
    {
        leftKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        middleKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rightKnob.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private IEnumerator Bake()
    {
        if(IsServer)
            {
                StationResultClientRPC(true);
            } else {
                StationResultServerRPC(true);
            }
        timerActive = true;
        ovenSFX = AudioManager.Instance.PlayLoopingSFX(FMODEvents.NetworkSFXName.StationTicking);
        timerObject.SetActive(true);
        timerMaterial.SetFloat("_Border_Thickness", 1);
        timerMaterial.SetTexture("_Texture", resultingItem.itemSprite.texture);
        timerMaterial.EnableKeyword("_USE_TEXTURE");

        yield return new WaitForSeconds(bakeTime);
        ovenSFX.stop(STOP_MODE.ALLOWFADEOUT);
    
        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.CompleteOrder, transform.position);
        timerMaterial.SetFloat("_Border_Thickness", 0.3f);
        Debug.Log("baked");
        itemReady = true;

        StartCoroutine(Spoil());

    }

    private IEnumerator Spoil()
    {
        yield return new WaitForSeconds(timeBeforeExpire);

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

        timerActive = false;
        resultingItem = null;
    }

    //change isBaking
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

}
