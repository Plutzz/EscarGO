using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Airborne", menuName = "Player Logic/Airborne Logic/Default")]
public class PlayerAirborneVinh : PlayerAirborneSOBase
{

    [SerializeField] private float upwardGravityAcceleration;   //When the player is moving upward there will be less gravity applied
    [SerializeField] private float downwardGravityAcceleration; //When the player is moving downward there will be more gravity applied

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine, PlayerInputActions playerInputActions)
    {
        base.Initialize(gameObject, stateMachine, playerInputActions);
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
        base.DoFixedUpdateState();
        if (rb.velocity.y > 0)
        {
            rb.velocity += Time.fixedDeltaTime * upwardGravityAcceleration * Vector3.down;
        }
        else {
            rb.velocity += downwardGravityAcceleration * Time.fixedDeltaTime * Vector3.down;
        }

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
        // Airborne => Moving
        if (stateMachine.GroundedCheck() && playerInputActions.Player.Move.ReadValue<Vector2>() != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        // Airborne => Idle
        else if (stateMachine.GroundedCheck() && playerInputActions.Player.Move.ReadValue<Vector2>() == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        } 

    }
}