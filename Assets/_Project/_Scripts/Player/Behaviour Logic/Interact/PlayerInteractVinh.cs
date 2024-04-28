using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(fileName = "Interact", menuName = "Player Logic/Interact Logic/Default")]
public class PlayerInteractVinh : PlayerInteractSOBase
{
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        stateMachine.inputManager.SwitchActionMap("MiniGames");
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        stateMachine.inputManager.SwitchActionMap("Player");
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
}