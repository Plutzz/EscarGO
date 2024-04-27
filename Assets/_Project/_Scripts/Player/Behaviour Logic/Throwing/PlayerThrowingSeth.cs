using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Throwing", menuName = "Player Logic/Throwing Logic/Default")]
public class PlayerThrowingSeth : PlayerThrowingSOBase
{
    [Header("Movement Variables")]
    private Vector3 moveDirection = Vector3.zero;

    private EventInstance currentFootstepSFXInstance;

    [SerializeField] private float moveSpeedMultiplier = .5f;
    [SerializeField] private float timeToChargeThrow = .75f;
    private float chargeTimer = 0;

    [SerializeField] private FoodProjectile projectile;

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }
    public override void DoEnterLogic()
    {
        Debug.Log("Entered throwing state");
        base.DoEnterLogic();
        chargeTimer = 0;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        currentFootstepSFXInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public override void DoFixedUpdateState()
    {
        Move();

        base.DoFixedUpdateState();
    }

    public override void DoUpdateState()
    {
        
        chargeTimer += Time.deltaTime;
       
        GetInput();
        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
    public override void CheckTransitions()
    {
        // Throwing => Airborne
        
        if (!stateMachine.TryingThrow())
        {
            if (chargeTimer > timeToChargeThrow) {
                Throw();
                chargeTimer = float.MinValue;
            }
            stateMachine.ChangeState(stateMachine.MovingState);
        }
        

    }

    #region Helper Methods

    private void GetInput()
    {
        inputVector = stateMachine.inputManager.MoveInput;
    }

    private void Move()
    {
        moveDirection = (stateMachine.orientation.forward * inputVector.y + stateMachine.orientation.right * inputVector.x).normalized;
        rb.velocity = new Vector3(moveDirection.x * stateMachine.moveSpeed, rb.velocity.y, moveDirection.z * stateMachine.moveSpeed ) * moveSpeedMultiplier;
    }

    #endregion
    private void Throw() {
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.PlayerThrow, rb.transform.position);
        stateMachine.GetComponent<PlayerProjectileManager>().ThrowProjectileServerRpc(stateMachine.projectilePosition.position, stateMachine.cameraTransform.rotation);
        return;

        if (playerInventory == null)
        {
            
            return;
        }
        else if (playerInventory.CurrentlyHasItem() == false)
        {
            //return;
        }
        else { 
            playerInventory.RemoveActiveItem();
        }
        FoodProjectile newProjectile = Instantiate(projectile, gameObject.transform.position + stateMachine.orientation.forward, gameObject.transform.rotation);
        //newProjectile.Launch();

    }
}