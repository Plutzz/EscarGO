using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowingSOBase : PlayerStateSOBase
{
    protected PlayerStateMachine stateMachine;
    protected Rigidbody rb;
    protected GameObject gameObject;
    protected Vector2 inputVector;
    protected PlayerInventory playerInventory;

    public virtual void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
        rb = stateMachine.rb;
        playerInventory = gameObject.GetComponentInChildren<PlayerInventory>();

    }

    public override void CheckTransitions()
    {

        // Moving => Airborne
        if (!stateMachine.GroundedCheck())
        {
            stateMachine.ChangeState(stateMachine.AirborneState);
        }
        // Moving => Idle
        else if (stateMachine.inputManager.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}