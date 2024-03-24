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
            // Render the player model if it is not the local client's player
            graphics.layer = LayerMask.NameToLayer("Player");
            foreach(Transform child in graphics.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            return;
        }
        anim = GetComponentInChildren<ClientNetworkAnimator>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerInputActions = playerStateMachine.inputManager;
    }

    void Update()
    {
    }


    public void HandleAnimations(PlayerState _state)
    {
        Debug.Log("Handling Animations: " + _state);
        anim.ResetTrigger("Dance");
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Jogging");
        anim.ResetTrigger("Cooking");
        switch (_state)
        {
            case PlayerAirborneState _:
                if (playerInputActions.MoveInput != Vector2.zero && playerInputActions.SprintIsPressed)
                    anim.SetTrigger("Dance");
                else if (playerInputActions.MoveInput != Vector2.zero)
                    anim.SetTrigger("Jogging");
                else
                    anim.SetTrigger("Idle");
                break;
            case PlayerMovingState _:
                if (playerInputActions.SprintIsPressed)
                    anim.SetTrigger("Dance");
                else
                    anim.SetTrigger("Jogging");
                break;
            case PlayerInteractState _:
                anim.SetTrigger("Cooking");
                break;
            case PlayerIdleState _:
                anim.SetTrigger("Idle");
                break;
        }
    }
}
