using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Idle", menuName = "Player Logic/Idle Logic/Default")]
public class PlayerIdleVinh : PlayerIdleSOBase
{
    [SerializeField] private float groundDrag;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        rb.velocity = Vector3.zero;
        rb.drag = groundDrag;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        rb.drag = 0;
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    public override void CheckTransitions()
    {
        // Idle => Airborne
        if (!stateMachine.GroundedCheck() && !stateMachine.crouching)
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        // Idle => Moving
        else if (stateMachine.inputManager.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        else if (stateMachine.TryingThrow())
        {
            Debug.Log("Moving to throw state");
            stateMachine.ChangeState(stateMachine.ThrowingState);
        }
    }
}