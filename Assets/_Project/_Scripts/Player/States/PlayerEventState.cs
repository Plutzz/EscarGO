using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventState : PlayerState
{
    public PlayerEventState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterLogic()
    {
        stateMachine.PlayerEventBaseInstance.DoEnterLogic();
    }

    public override void ExitLogic()
    {
        stateMachine.PlayerEventBaseInstance.DoExitLogic();
    }
    public override void UpdateState()
    {
        stateMachine.PlayerEventBaseInstance.DoUpdateState();
    }

    public override void FixedUpdateState()
    {
        stateMachine.PlayerEventBaseInstance.DoFixedUpdateState();
    }
}
