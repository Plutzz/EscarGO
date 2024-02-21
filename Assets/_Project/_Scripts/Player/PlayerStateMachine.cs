using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerStateMachine : MonoBehaviour
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
    public PlayerInputActions playerInputActions { get; private set; }
    public Rigidbody rb { get; private set; }

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


    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();

        PlayerIdleBaseInstance = Instantiate(playerIdleBase);
        PlayerMovingBaseInstance = Instantiate(playerMovingBase);
        PlayerAirborneBaseInstance = Instantiate(playerAirborneBase);
        PlayerInteractBaseInstance = Instantiate(playerInteractBase);

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();


        IdleState = new PlayerIdleState(this);
        MovingState = new PlayerMovingState(this);
        AirborneState = new PlayerAirborneState(this);
        InteractState = new PlayerInteractState(this);

        PlayerIdleBaseInstance.Initialize(gameObject, this, playerInputActions);
        PlayerMovingBaseInstance.Initialize(gameObject, this, playerInputActions);
        PlayerAirborneBaseInstance.Initialize(gameObject, this, playerInputActions);
        PlayerInteractBaseInstance.Initialize(gameObject, this, playerInputActions);

        initialState = IdleState;
        startYScale = gameObject.transform.localScale.y;
    }




    public void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        currentState = initialState;
        currentState.EnterLogic();
    }


    private void Update()
    {
        crouching = playerInputActions.Player.Crouch.ReadValue<float>() == 1f;
        
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