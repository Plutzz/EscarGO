using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowingState : PlayerState
{
    public PlayerThrowingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterLogic()
    {
        stateMachine.PlayerThrowingBaseInstance.DoEnterLogic();
    }

    public override void ExitLogic()
    {
        stateMachine.PlayerThrowingBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        stateMachine.PlayerThrowingBaseInstance.DoUpdateState();
    }

    public override void FixedUpdateState()
    {
        stateMachine.PlayerThrowingBaseInstance.DoFixedUpdateState();
    }
}
