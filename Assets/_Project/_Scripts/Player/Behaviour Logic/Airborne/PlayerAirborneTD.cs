using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AirborneTD", menuName = "Player Logic/Airborne Logic/Top Down")]
public class PlayerAirborneTD: PlayerAirborneSOBase
{

    [SerializeField] private float speed = 5f;
    private Vector2 input;

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
        if (stateMachine.GroundedCheck() && stateMachine.inputManager.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        // Airborne => Idle
        else if (stateMachine.GroundedCheck() && stateMachine.inputManager.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    private void GetInput()
    {
        input = stateMachine.inputManager.MoveInput;
    }

    private void Move()
    {
        if (input == Vector2.zero)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        float speed = this.speed;
        if (stateMachine.inputManager.SprintIsPressed)
        {
            speed = speed * 2;
        }

        rb.velocity = new Vector3(input.x * speed, rb.velocity.y, input.y * speed);
    }
}