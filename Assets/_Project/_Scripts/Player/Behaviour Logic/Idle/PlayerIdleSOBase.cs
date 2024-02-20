using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleSOBase : PlayerStateSOBase
{
    protected PlayerStateMachine stateMachine;
    protected Rigidbody rb;
    protected GameObject gameObject;
    protected PlayerInputActions playerInputActions;

    public virtual void Initialize(GameObject gameObject, PlayerStateMachine stateMachine, PlayerInputActions playerInputActions)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
        rb = stateMachine.rb;
        this.playerInputActions = playerInputActions;
    }
    public override void CheckTransitions()
    {
        if (!stateMachine.GroundedCheck() && !stateMachine.crouching)
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        else if (playerInputActions.Player.Movement.ReadValue<Vector2>() != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
    }


}