using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerState
{
    public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterLogic()
    {
        stateMachine.PlayerInteractBaseInstance.DoEnterLogic();
    }

    public override void ExitLogic()
    {
        stateMachine.PlayerInteractBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        stateMachine.PlayerInteractBaseInstance.DoUpdateState();
    }

    public override void FixedUpdateState()
    {
        stateMachine.PlayerInteractBaseInstance.DoFixedUpdateState();
    }
}
