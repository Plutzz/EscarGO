using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Idle", menuName = "Player Logic/Idle Logic/Default")]
public class PlayerIdleVinh : PlayerIdleSOBase
{
    //[SerializeField] private float groundDrag;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        rb.velocity = Vector3.zero;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
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
    }
}