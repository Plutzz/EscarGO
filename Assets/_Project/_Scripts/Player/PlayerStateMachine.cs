using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;


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


    #region ScriptableObject Variables

    [SerializeField] private PlayerIdleSOBase playerIdleBase;
    [SerializeField] private PlayerMovingSOBase playerMovingBase;
    [SerializeField] private PlayerAirborneSOBase playerAirborneBase;
    [SerializeField] private PlayerInteractSOBase playerInteractBase;

    public PlayerIdleSOBase PlayerIdleBaseInstance { get; private set; }
    public PlayerMovingSOBase PlayerMovingBaseInstance { get; private set; }
    public PlayerAirborneSOBase PlayerAirborneBaseInstance { get; private set; }
    public PlayerInteractSOBase PlayerInteractBaseInstance { get; private set; }

    #endregion

    #region Player Variables
    public Rigidbody rb { get; private set; }
    public InputManager inputManager { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight;
    public float moveSpeed = 5f;
    public Transform cameraTransform;
    public Transform player;

    [Header("Crouching Variables")]
    [SerializeField] private float crouchYScale = 0.5f;
    private float startYScale;
    public bool crouching { get; private set; }


    #endregion


    public override void OnNetworkSpawn()
    {
        // If this script is not owned by the client
        // Delete it so no input is picked up by it
        if (!IsOwner)
            Destroy(this);

        rb = GetComponentInChildren<Rigidbody>();
        inputManager = GetComponent<InputManager>();

        PlayerIdleBaseInstance = Instantiate(playerIdleBase);
        PlayerMovingBaseInstance = Instantiate(playerMovingBase);
        PlayerAirborneBaseInstance = Instantiate(playerAirborneBase);
        PlayerInteractBaseInstance = Instantiate(playerInteractBase);


        IdleState = new PlayerIdleState(this);
        MovingState = new PlayerMovingState(this);
        AirborneState = new PlayerAirborneState(this);
        InteractState = new PlayerInteractState(this);

        PlayerIdleBaseInstance.Initialize(gameObject, this);
        PlayerMovingBaseInstance.Initialize(gameObject, this);
        PlayerAirborneBaseInstance.Initialize(gameObject, this);
        PlayerInteractBaseInstance.Initialize(gameObject, this);

        initialState = IdleState;
        startYScale = gameObject.transform.localScale.y;
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
            player.localScale = new Vector3(player.localScale.x, crouchYScale, gameObject.transform.localScale.z);
        else
            player.localScale = new Vector3(player.localScale.x, startYScale, gameObject.transform.localScale.z);

        currentState.UpdateState();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(PlayerState newState)
    {
        Debug.Log("Changing to: " + newState);
        currentState.ExitLogic();
        previousState = currentState;
        currentState = newState;
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

    #endregion

}