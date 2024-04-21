using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventSOBase : PlayerStateSOBase
{
    protected PlayerStateMachine stateMachine;
    protected Rigidbody rb;
    protected GameObject gameObject;
    protected Vector2 inputVector;

    public virtual void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
        rb = stateMachine.rb;

    }

    public override void CheckTransitions()
    {
        // Event => Airborne
        if (!stateMachine.GroundedCheck())
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        // Event => Moving
        else if (stateMachine.inputManager.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        // Event => Interact
        else if (stateMachine.inputManager.InteractIsPressed)
        {
            stateMachine.ChangeState(stateMachine.InteractState);
        }
        // Event => Idle
        else if (stateMachine.inputManager.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }
}
