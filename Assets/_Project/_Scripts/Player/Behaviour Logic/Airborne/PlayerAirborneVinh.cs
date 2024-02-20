using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Airborne", menuName = "Player Logic/Airborne Logic/Default")]
public class PlayerAirborneVinh : PlayerAirborneSOBase
{
    [SerializeField] private float acceleration = 40f; // player is only able to decelerate during this state
    [SerializeField] private float drag = 0f;

    [SerializeField] private float upwardGravityAcceleration;   //When the player is moving upward there will be less gravity applied
    [SerializeField] private float downwardGravityAcceleration; //When the player is moving downward there will be more gravity applied

    [SerializeField] private float sprintMovementMultiplier;
    [SerializeField] private float apexMovementMultiplier;
    [SerializeField] private float apexYVelocityThreshold;

    [Header("Collision Variables")]
    [SerializeField] private float collisionAcceleration;
    [SerializeField] private float collisionSpeed;
    [Tooltip("Time Before collisions start slowing down the player")]
    [SerializeField] private float collisionTimer = 0.5f;
    private float collisionTime = 0f;
    private bool sprinting;
    private float speedOnEnter; // speed while entering the state

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine, PlayerInputActions playerInputActions)
    {
        base.Initialize(gameObject, stateMachine, playerInputActions);
    }
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        stateMachine.StopAllCoroutines();
        rb.drag = drag;
        speedOnEnter = stateMachine.moveSpeed;

        rb.useGravity = false;
        collisionTime = collisionTimer;
    }

    public override void DoExitLogic()
    {
        rb.useGravity = true;
        base.DoExitLogic();
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
        Move();

        if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.down * upwardGravityAcceleration * Time.fixedDeltaTime;
        }
        else {
            rb.velocity += Vector3.down * downwardGravityAcceleration * Time.fixedDeltaTime;
        }

    }

    public override void DoUpdateState()
    {
        GetInput();
        MovementSpeedHandler();

        collisionTime -= Time.deltaTime;

        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        speedOnEnter = 0f;
        base.ResetValues();
    }

    private void GetInput()
    {
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        sprinting = playerInputActions.Player.Sprint.ReadValue<float>() != 0;
    }
    private void MovementSpeedHandler()
    {

        stateMachine.StopCoroutine(stateMachine.SmoothlyLerpMoveSpeed(collisionAcceleration));
        stateMachine.desiredMoveSpeed = speedOnEnter;
        stateMachine.moveSpeed = stateMachine.desiredMoveSpeed;
        
        stateMachine.lastDesiredMoveSpeed = stateMachine.desiredMoveSpeed;
    }

    // Todo - CHANGE THIS TO A VELOCITY BASED SYSTEM
    private void Move()
    {
        if (inputVector == Vector2.zero) { return; }

        Vector3 _moveDir = stateMachine.player.forward * inputVector.y + stateMachine.player.right * inputVector.x;
        
        float forceMultiplier = 1;

        if (sprinting) {
            forceMultiplier *= sprintMovementMultiplier;
        }

        if (Mathf.Abs(rb.velocity.y) < apexYVelocityThreshold) {
            forceMultiplier *= apexMovementMultiplier;
        }

        rb.AddForce(_moveDir.normalized * acceleration * forceMultiplier, ForceMode.Force);
    }

    public override void CheckTransitions()
    {
        // Airborne => Moving
        if (stateMachine.GroundedCheck() && playerInputActions.Player.Movement.ReadValue<Vector2>() != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        // Airborne => Idle
        else if (stateMachine.GroundedCheck() && playerInputActions.Player.Movement.ReadValue<Vector2>() == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        } 

    }
}