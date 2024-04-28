using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;


[CreateAssetMenu(fileName = "Interact", menuName = "Player Logic/Interact Logic/Default")]
public class PlayerInteractVinh : PlayerInteractSOBase
{
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        stateMachine.inputManager.SwitchActionMap("MiniGames");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().DisablePrompts();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().ClearUIItem();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().crosshair.SetActive(false);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        stateMachine.inputManager.SwitchActionMap("Player");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ButtonPromptCheck>().EnablePrompts();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().crosshair.SetActive(true);
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