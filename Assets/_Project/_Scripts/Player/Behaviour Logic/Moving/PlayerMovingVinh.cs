using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Moving", menuName = "Player Logic/Moving Logic/Default")]
public class PlayerMovingVinh : PlayerMovingSOBase
{
    [Header("Movement Variables")]
    private Vector3 moveDirection = Vector3.zero;


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
        else if (InputManager.Instance.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }

    #region Helper Methods

    private void GetInput()
    {
        inputVector = InputManager.Instance.MoveInput;
    }

    private void Move()
    {
        moveDirection = (stateMachine.cameraTransform.forward * inputVector.y + stateMachine.cameraTransform.right * inputVector.x).normalized;
        rb.velocity = new Vector3(moveDirection.x * stateMachine.moveSpeed, rb.velocity.y, moveDirection.z * stateMachine.moveSpeed);
    }

    #endregion

}