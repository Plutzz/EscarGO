using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;
using Cinemachine;


public class PlayerStateMachine : NetworkBehaviour
{
    public PlayerState currentState { get; private set; } // State that player is currently in
    private PlayerState initialState; // State that player starts as
    public PlayerState previousState { get; private set; } // State that player starts as

    // References to all player states
    public PlayerIdleState IdleState;
    public PlayerMovingState MovingState;
    public PlayerAirborneState AirborneState;
    public PlayerInteractState InteractState;
    public PlayerThrowingState ThrowingState;
    public PlayerEventState EventState;


    #region ScriptableObject Variables

    [SerializeField] private PlayerIdleSOBase playerIdleBase;
    [SerializeField] private PlayerThrowingSOBase playerMovingBase;
    [SerializeField] private PlayerAirborneSOBase playerAirborneBase;
    [SerializeField] private PlayerInteractSOBase playerInteractBase;
    [SerializeField] private PlayerThrowingSOBase playerThrowingBase;
    [SerializeField] private PlayerEventSOBase playerEventBase;

    public PlayerIdleSOBase PlayerIdleBaseInstance { get; private set; }
    public PlayerThrowingSOBase PlayerMovingBaseInstance { get; private set; }
    public PlayerAirborneSOBase PlayerAirborneBaseInstance { get; private set; }
    public PlayerInteractSOBase PlayerInteractBaseInstance { get; private set; }
    public PlayerThrowingSOBase PlayerThrowingBaseInstance { get; private set; }
    public PlayerEventSOBase PlayerEventBaseInstance { get; private set; }

    #endregion

    #region Player Variables
    public Rigidbody rb { get; private set; }
    public InputManager inputManager { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight;
    public float moveSpeed = 5f;
    public Transform cameraTransform;
    public Transform orientation;
    public Transform player;

    public Transform projectilePosition;
    private PlayerAnim playerAnim;
    [Header("Crouching Variables")]
    [SerializeField] private float crouchYScale = 0.5f;
    private float startYScale;
    public bool crouching { get; private set; }
    public CinemachineVirtualCamera cam;
    public float initialFOV { get; private set; }

    #endregion


    public override void OnNetworkSpawn()
    {
        // If this script is not owned by the client
        // Delete it so no input is picked up by it
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
            

        rb = GetComponentInChildren<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        playerAnim = GetComponent<PlayerAnim>();

        PlayerIdleBaseInstance = Instantiate(playerIdleBase);
        PlayerMovingBaseInstance = Instantiate(playerMovingBase);
        PlayerAirborneBaseInstance = Instantiate(playerAirborneBase);
        PlayerInteractBaseInstance = Instantiate(playerInteractBase);
        PlayerThrowingBaseInstance = Instantiate(playerThrowingBase);
        PlayerEventBaseInstance = Instantiate(playerEventBase);


        IdleState = new PlayerIdleState(this);
        MovingState = new PlayerMovingState(this);
        AirborneState = new PlayerAirborneState(this);
        InteractState = new PlayerInteractState(this);
        ThrowingState = new PlayerThrowingState(this);
        EventState = new PlayerEventState(this);

        PlayerIdleBaseInstance.Initialize(gameObject, this);
        PlayerMovingBaseInstance.Initialize(gameObject, this);
        PlayerAirborneBaseInstance.Initialize(gameObject, this);
        PlayerInteractBaseInstance.Initialize(gameObject, this);
        PlayerThrowingBaseInstance.Initialize(gameObject, this);
        PlayerEventBaseInstance.Initialize(gameObject, this);

        initialState = IdleState;
        startYScale = gameObject.transform.localScale.y;

        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        initialFOV = cam.m_Lens.FieldOfView;
    }




    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentState = initialState;
        currentState.EnterLogic();
    }


    private void Update()
    {
        crouching = GetComponent<InputManager>().CrouchIsPressed;
        
        if (crouching)
        {
            rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            player.localScale = new Vector3(player.localScale.x, crouchYScale, gameObject.transform.localScale.z);
        }
        else
        {
            player.localScale = new Vector3(player.localScale.x, startYScale, gameObject.transform.localScale.z);
        }

            

        currentState.UpdateState();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(PlayerState newState)
    {
        if (!IsOwner)
            return;

        Debug.Log("Changing to: " + newState);
        currentState.ExitLogic();
        previousState = currentState;
        currentState = newState;
        playerAnim.HandleAnimations(currentState);
        currentState.EnterLogic();
    }


    #region Logic Checks

    //Consider adding core functionalities here
    // Ex: GroundedCheck
    public bool GroundedCheck()
    {
        Debug.DrawRay(transform.position, Vector3.down * playerHeight * 0.5f + Vector3.down * 0.2f);
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    public bool TryingThrow() {
        return inputManager.JumpIsPressed;
    }

    public void Stunned()
    {
        ChangeState(EventState);
    }

    #endregion

    #region Utility

    public void LerpFOV(float finalFOV, float time) { 
        StartCoroutine(ChangeFOV(finalFOV, time));
    }

    IEnumerator ChangeFOV(float finalFOV, float totalTime) {
        float timer = 0;
        float currentFOV = cam.m_Lens.FieldOfView;

        while (timer < totalTime) { 
            cam.m_Lens.FieldOfView = Mathf.Lerp(currentFOV, finalFOV, timer/ totalTime);
            
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        cam.m_Lens.FieldOfView = finalFOV;
    }

    #endregion
}