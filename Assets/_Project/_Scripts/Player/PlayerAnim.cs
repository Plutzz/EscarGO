using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;
using Unity.Netcode.Components;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
public class PlayerAnim : NetworkBehaviour
{
    public GameObject graphics;
    public NetworkAnimator anim { get; private set; }
    public PlayerStateMachine playerStateMachine;
    public PlayerState currentState;
    private InputManager playerInputActions;

    public override void OnNetworkSpawn()
    {
        // If this script is not owned by the client
        // Delete it so no input is picked up by it
        if (!IsOwner)
        {
            SetVisibleLayer(graphics.transform);
            // Render the player model if it is not the local client's player
            graphics.layer = LayerMask.NameToLayer("Default");
            return;
        }
        anim = GetComponentInChildren<ClientNetworkAnimator>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerInputActions = playerStateMachine.inputManager;
    }

    private void SetVisibleLayer(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
            if (child.childCount > 0)
            {
                SetVisibleLayer(child);
            }
        }

    }

    void Update()
    {
    }


    public void HandleAnimations(PlayerState _state)
    {
        Debug.Log("Handling Animations: " + _state);
        anim.ResetTrigger("Run");
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Jogging");
        anim.ResetTrigger("Cooking");
        switch (_state)
        {
            case PlayerAirborneState _:
                if (playerInputActions.MoveInput != Vector2.zero && playerInputActions.SprintIsPressed)
                    anim.SetTrigger("Run");
                else if (playerInputActions.MoveInput != Vector2.zero)
                    anim.SetTrigger("Jogging");
                else
                    anim.SetTrigger("Idle");
                break;
            case PlayerMovingState _:
                if (playerInputActions.SprintIsPressed)
                    anim.SetTrigger("Run");
                else
                    anim.SetTrigger("Jogging");
                break;
            case PlayerInteractState _:
                anim.SetTrigger("Cooking");
                break;
            case PlayerEventState _:
                break;
            case PlayerIdleState _:
                anim.SetTrigger("Idle");
                break;
        }
    }
}
