using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    # region Object References
    [Header("Object References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight;
    [SerializeField] private Transform orientation;
    [SerializeField] private TextMeshPro nameTag;
    [SerializeField] private GameObject graphics;
    private PlayerStateMachine stateMachine;
    private PlayerState currentState;
    private Rigidbody rb;
    private InputManager inputManager;
    #endregion

    # region Stamina Variables
    [Header("Stamina Variables")]
    [SerializeField] private float maxStamina = 100f;
    public float currentStamina;
    public float staminaDecreaseRate = 10f;
    public float staminaIncreaseRate = 5f;
    private float sprintSpeed = 10f;
    private float moveSpeed;
    private bool sprinting = false;
    public bool canSprint = true;
    #endregion

    # region Jumping Variables
    [Header("Jumping Variables")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldownDuration = 1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpCooldown;
    public bool canJump = true;
    private float lastJumpPressed;

    #endregion
    [SerializeField] private SettingsMenu pauseMenu;
    [SerializeField] private FirstPersonCamera cameraScript;
    [SerializeField] private Vector3 spawnPos;

    [Header("Spectate Variables")]
    [SerializeField] private Vector3 localCameraPosition;
    [SerializeField] private List<GameObject> toggledGameObjects;
    [SerializeField] private List<MonoBehaviour> toggledMonoBehaviours;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            enabled = false;
            return;
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            SetupPlayerName();
        }

        stateMachine = GetComponent<PlayerStateMachine>();
        moveSpeed = stateMachine.moveSpeed;
        rb = stateMachine.rb;
        currentStamina = maxStamina;
        canJump = true;
        graphics.SetActive(false);
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        rb.velocity = Vector3.zero;
        rb.position = spawnPos;
    }
    private async void SetupPlayerName()
    {
        await AuthenticationService.Instance.GetPlayerNameAsync();

        string _playerName = AuthenticationService.Instance.PlayerName;

        gameObject.name = _playerName;
        nameTag.text = _playerName;
    }

    void Update()
    {
        sprinting = stateMachine.inputManager.SprintIsPressed;

        currentState = stateMachine.currentState;

        if (jumpCooldown <= Time.time)
        {
            canJump = true;
        }

        if (stateMachine.inputManager.JumpPressedThisFrame)
        {
            lastJumpPressed = Time.time;
            if (currentState == stateMachine.AirborneState) return;

            if (canJump && lastJumpPressed + jumpBufferTime >= Time.time)
            {
                GroundedJump();
            }
        }

        if (currentState == stateMachine.AirborneState) return;
        

        if (sprinting && canSprint)
        {
            DecreaseStamina();
        }
        else
        {
            IncreaseStamina();
        }

        if(inputManager.PausePressedThisFrame && pauseMenu != null)
        {
            pauseMenu.OpenMenu();
        }

        if(orientation != null && cameraScript != null)
            orientation.eulerAngles = new Vector3 (0f, cameraScript.transform.eulerAngles.y, 0f);

    }


    private void DecreaseStamina()
    {
        currentStamina -= staminaDecreaseRate * Time.deltaTime;
        stateMachine.moveSpeed = sprintSpeed;
        currentStamina = Mathf.Max(currentStamina, 0);
        if (currentStamina <= 0)
        {
            canSprint = false;
        }
    }

    private void IncreaseStamina()
    {
        currentStamina += staminaIncreaseRate * Time.deltaTime;
        stateMachine.moveSpeed = moveSpeed;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
        if (currentStamina < maxStamina * 0.5f && currentStamina > 0)
            canSprint = false;
        else
            canSprint = true;

    }

    private void GroundedJump()
    {
        canJump = false;
        jumpCooldown = Time.time + jumpCooldownDuration;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JumpSound, transform.position);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [ContextMenu("Spectate")]
    public void Spectate(bool isSpectating) {
        stateMachine.cameraTransform.localPosition = localCameraPosition;

        foreach(GameObject gameObject in toggledGameObjects){ 
            gameObject.SetActive(!isSpectating);
        }

        foreach (MonoBehaviour thing in toggledMonoBehaviours) { 
            thing.enabled = !isSpectating;
        }

    
    }
}
