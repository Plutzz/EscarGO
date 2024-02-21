using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Moving", menuName = "Player Logic/Moving Logic/Default")]
public class PlayerMovingVinh : PlayerMovingSOBase
{
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float groundDrag = 1f;
    private bool sprinting = false;
    private Vector3 moveDirection;


    [Header("Camera Variables")]
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    private Vector2 mouseDirection;

    [SerializeField]
    private float m_CurrentUpDown = 0;
    private Vector2 m_CurrentMovement = Vector2.zero;
    private Vector2 m_CurrentLooking = Vector2.zero;

    [Header("Crouching Variables")]
    [SerializeField] private float crouchSpeed = 3.5f;


    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine, PlayerInputActions playerInputActions)
    {
        base.Initialize(gameObject, stateMachine, playerInputActions);
    }
    public override void DoEnterLogic()
    {
        moveDirection = Vector3.zero;
        rb.drag = groundDrag;
        stateMachine.StopAllCoroutines();
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFixedUpdateState()
    {
        GetInput();
        Move();
        

        base.DoFixedUpdateState();
    }

    public override void DoUpdateState()
    {
        MoveCamera();
        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
    public override void CheckTransitions()
    {
        // Moving => Airborne
        if (!stateMachine.GroundedCheck())
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        // Moving => Idle
        else if (playerInputActions.Player.Movement.ReadValue<Vector2>() == Vector2.zero && rb.velocity.magnitude < 3f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    #region Helper Methods


    private void GetInput()
    {
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        sprinting = playerInputActions.Player.Sprint.ReadValue<float>() == 1f;
        mouseDirection = playerInputActions.Player.Look.ReadValue<Vector2>();
    }

    // Todo - CHANGE TO VELOCITY BASED SYSTEM
    private void Move()
    {
        moveDirection = (stateMachine.cameraTransform.forward * inputVector.y + stateMachine.cameraTransform.right * inputVector.x).normalized;
        print(moveDirection);
        rb.velocity = new Vector3(moveDirection.x * stateMachine.moveSpeed, rb.velocity.y, moveDirection.z * stateMachine.moveSpeed);
    }

    private void MoveCamera()
    {
        m_CurrentLooking += mouseDirection * sensitivity;
        m_CurrentLooking.y = Mathf.Clamp(m_CurrentLooking.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(m_CurrentLooking.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(m_CurrentLooking.y, Vector3.left);
        stateMachine.cameraTransform.localRotation = xQuat * yQuat;
    }


    #endregion

}