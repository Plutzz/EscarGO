using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    # region Object References
    [Header("Object References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight;
    [SerializeField] private Transform orientation;
    private PlayerStateMachine stateMachine;
    private PlayerState currentState;
    private Rigidbody rb;
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

    [Header("Temp Debug Variables")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private FirstPersonCamera cameraScript;


    void Start()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        cameraScript = GetComponentInChildren<FirstPersonCamera>();
        moveSpeed = stateMachine.moveSpeed;
        rb = stateMachine.rb;
        currentStamina = maxStamina;
        canJump = true;
    }

    void Update()
    {
        sprinting = InputManager.Instance.SprintIsPressed;

        currentState = stateMachine.currentState;

        if (jumpCooldown <= Time.time)
        {
            canJump = true;
        }

        if (InputManager.Instance.JumpPressedThisFrame)
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

        if(Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null)
        {
            if (!pauseMenu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                pauseMenu.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.SetActive(false);
            }
        }

        if(orientation != null)
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

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
