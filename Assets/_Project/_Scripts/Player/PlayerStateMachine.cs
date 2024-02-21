using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
// using Unity.Netcode.Components;
// using Unity.Netcode;
using TMPro;


public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState { get; private set; } // State that player is currently in
    private PlayerState initialState; // State that player starts as
    public PlayerState previousState { get; private set; } // State that player starts as

    // References to all player states
    public PlayerIdleState IdleState;
    public PlayerMovingState MovingState;
    public PlayerAirborneState AirborneState;


    #region ScriptableObject Variables

    [SerializeField] private PlayerIdleSOBase playerIdleBase;
    [SerializeField] private PlayerMovingSOBase playerMovingBase;
    [SerializeField] private PlayerAirborneSOBase playerAirborneBase;

    public PlayerIdleSOBase PlayerIdleBaseInstance { get; private set; }
    public PlayerMovingSOBase PlayerMovingBaseInstance { get; private set; }
    public PlayerAirborneSOBase PlayerAirborneBaseInstance { get; private set; }

    #endregion

    #region Player Variables
    public PlayerInputActions playerInputActions { get; private set; }
    public Rigidbody rb { get; private set; }
    // public NetworkAnimator animator { get; private set; }

    [SerializeField] private Vector3 startPos;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float playerHeight;
    [SerializeField] private GameObject playerCameraPrefab;

    public float moveSpeed;
    public float desiredMoveSpeed;
    public float lastDesiredMoveSpeed;

    [HideInInspector] public float timeOfLastJump;


    public Transform cameraTransform;
    public Transform player;
    public Transform playerHitbox;

    [Header("Crouching Variables")]
    [SerializeField] private float crouchYScale = 0.5f;
    private float startYScale;
    public bool crouching { get; private set; }




    // [Header("Switch Characters")]
    // [SerializeField] private Character[] characters;
    // [SerializeField] private CharacterNames currentCharacter;

    // [Serializable]
    // public struct Character
    // {
    //     [SerializeField] public CharacterNames name;
    //     [SerializeField] public Mesh characterMesh;
    //     [SerializeField] public AnimatorOverrideController animatorController;
    //     [SerializeField] public Material characterMaterial;
    //     [SerializeField] public Avatar characterAvatar;

    // }
    // [Serializable]
    // public enum CharacterNames
    // {

    // };

    #endregion
    #region Debug Variables
    public Vector3 RespawnPos;
    public float teleportAmount;
    #endregion

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();

        PlayerIdleBaseInstance = Instantiate(playerIdleBase);
        PlayerMovingBaseInstance = Instantiate(playerMovingBase);
        PlayerAirborneBaseInstance = Instantiate(playerAirborneBase);

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        // playerInputActions.Player.Crouch.performed += StartCrouch;
        // playerInputActions.Player.Crouch.canceled += StopCrouch;
        // playerInputActions.Player.Sprint.performed += StartSprint;
        // playerInputActions.Player.Sprint.canceled += StopSprint;


        IdleState = new PlayerIdleState(this);
        MovingState = new PlayerMovingState(this);
        AirborneState = new PlayerAirborneState(this);


        PlayerIdleBaseInstance.Initialize(gameObject, this, playerInputActions);
        PlayerMovingBaseInstance.Initialize(gameObject, this, playerInputActions);
        PlayerAirborneBaseInstance.Initialize(gameObject, this, playerInputActions);

        initialState = IdleState;
        startYScale = gameObject.transform.localScale.y;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

    }

    # region Network Functions
    // public override void OnNetworkSpawn()
    // {
    //     // Set up client side objects (Camera, debug menu)
    //     if (IsOwner)
    //     {
    //         transform.position = startPos;

    //         // animator = GetComponentInChildren<NetworkAnimator>();

    //         // CinemachineFreeLook playerCamera = Instantiate(playerCameraPrefab.GetComponent<CinemachineFreeLook>(), transform);
    //         // playerCamera.m_LookAt = transform;
    //         // playerCamera.m_Follow = transform;



    //         currentState = initialState;
    //         currentState.EnterLogic();

    //         SwitchCharacters();
    //     }
    // }

    // public override void OnNetworkDespawn()
    // {
    //     playerInputActions.Player.Crouch.performed -= StartCrouch;
    //     playerInputActions.Player.Crouch.canceled -= StopCrouch;
    //     playerInputActions.Player.Sprint.performed -= StartSprint;
    //     playerInputActions.Player.Sprint.canceled -= StopSprint;

    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    #endregion

    public void Start()
    {
        TeleportPlayer(startPos);
        currentState = initialState;
        currentState.EnterLogic();
    }
    public void TeleportPlayer(Vector3 position)
    {
        transform.position = startPos;
    }



    private void Update()
    {
        // if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TeleportPlayer();
        }

        // if (Input.GetKeyDown(KeyCode.Y))
        // {
        //     SwitchCharacters();

        //     currentCharacter++;

        //     if ((int)currentCharacter >= characters.Length)
        //     {
        //         currentCharacter = 0;
        //     }

        // }

        //Crouching logic
        // crouching = playerInputActions.Player.Crouch.ReadValue<float>() == 1f;

        if (crouching)
            playerHitbox.localScale = new Vector3(playerHitbox.localScale.x, crouchYScale, gameObject.transform.localScale.z);
        else
            playerHitbox.localScale = new Vector3(playerHitbox.localScale.x, startYScale, gameObject.transform.localScale.z);
        currentState.UpdateState();
    }



    private void FixedUpdate()
    {
        // if (!IsOwner) return;

        currentState.FixedUpdateState();

    }

    public void ChangeState(PlayerState newState)
    {
        Debug.Log("Changing to: " + newState);
        currentState.ExitLogic();
        previousState = currentState;
        currentState = newState;
        HandleAnimations();
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

    private void StartCrouch(InputAction.CallbackContext context)
    {

        if (currentState == IdleState)
        {
            // animator.SetTrigger("Crouch Idle");
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    #endregion

    #region Debug Functions

    private void TeleportPlayer()
    {
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        moveSpeed = 0;
        desiredMoveSpeed = 0;
        lastDesiredMoveSpeed = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y + teleportAmount, transform.position.z);
    }

    private void ResetPlayer()
    {
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        moveSpeed = 0;
        desiredMoveSpeed = 0;
        lastDesiredMoveSpeed = 0;
        transform.position = RespawnPos;
    }


    #endregion

    #region Animations

    private void StartSprint(InputAction.CallbackContext context)
    {
        if (currentState == MovingState && !crouching)
        {
            // animator.SetTrigger("Running");
        }
    }

    private void StopSprint(InputAction.CallbackContext context)
    {
        if (currentState == MovingState)
        {
            // animator.SetTrigger("Jogging");
        }
    }
    private void StopCrouch(InputAction.CallbackContext context)
    {
        if (currentState == MovingState)
        {
            // animator.SetTrigger("Jogging");
        }
        else if (currentState == IdleState)
        {
            // animator.SetTrigger("Idle");
        }
    }

    private void HandleAnimations()
    {
        switch (currentState)
        {
            case PlayerIdleState _:

                if (crouching)
                {
                    // animator.SetTrigger("Crouch Idle");
                }
                else
                {
                    // animator.SetTrigger("Idle");
                }
                break;

            case PlayerAirborneState _:
                // animator.SetTrigger("Airborne");
                break;

            case PlayerMovingState _:

                if (crouching)
                {
                    // animator.SetTrigger("Crouch Walk");
                }
                else if (playerInputActions.Player.Sprint.ReadValue<float>() == 1)
                {
                    // animator.SetTrigger("Running");
                }
                else
                {
                    // animator.SetTrigger("Jogging");
                }

                break;
        }
    }

    // private void SwitchCharacters()
    // {
    //     //Switch character objects
    //     var _character = characters[(int)currentCharacter];

    //     //Switch animation controller
    //     GetComponentInChildren<Animator>().runtimeAnimatorController = _character.animatorController;

    //     //Switch Avatars
    //     GetComponentInChildren<Animator>().avatar = _character.characterAvatar;

    //     //Switch Mesh
    //     GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = _character.characterMesh;

    //     //Switch Material
    //     GetComponentInChildren<SkinnedMeshRenderer>().material = _character.characterMaterial;
    // }
    #endregion
}