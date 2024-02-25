using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Airborne", menuName = "Player Logic/Airborne Logic/Default")]
public class PlayerAirborneVinh : PlayerAirborneSOBase
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    private float turnSmoothVelocity;

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFixedUpdateState()
    {
        Move();
        base.DoFixedUpdateState();

    }

    public override void DoUpdateState()
    {
        GetInput();
        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }


    public override void CheckTransitions()
    {
        // Airborne => Moving
        if (stateMachine.GroundedCheck() && InputManager.Instance.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        // Airborne => Idle
        else if (stateMachine.GroundedCheck() && InputManager.Instance.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    private void GetInput()
    {
        inputVector = InputManager.Instance.MoveInput;
    }

    private void Move()
    {
        if (inputVector == Vector2.zero)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        float speed = this.speed;
        if (InputManager.Instance.SprintIsPressed)
        {
            speed = speed * 2;
        }

        Vector3 moveDir = (stateMachine.cameraTransform.forward * inputVector.y + stateMachine.cameraTransform.right * inputVector.x).normalized;
        rb.velocity = new Vector3(moveDir.x * speed, rb.velocity.y, moveDir.z * speed);
    }
}