using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovingTD", menuName = "Player Logic/Moving Logic/Top Down")]
public class PlayerMoveTD : PlayerMovingSOBase
{
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
        // Moving => Airborne
        if (!stateMachine.GroundedCheck())
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        // Moving => Idle
        else if (InputManager.Instance.MoveInput == Vector2.zero && rb.velocity.magnitude < 3f)
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
        rb.velocity = new Vector3(inputVector.x * stateMachine.moveSpeed, rb.velocity.y, inputVector.y * stateMachine.moveSpeed);
    }
}
