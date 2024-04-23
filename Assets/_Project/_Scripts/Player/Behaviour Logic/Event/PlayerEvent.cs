using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Event", menuName = "Player Logic/Event Logic/Default")]
public class PlayerEvent : PlayerEventSOBase
{
    [SerializeField] private float groundDrag;
    [SerializeField] private bool isStunned;
    [SerializeField] private float stunTime = 5.0f;
    private float stunDuration = 0.0f;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        stunDuration = stunTime;
        isStunned = true;
        rb.velocity = Vector3.zero;
        rb.drag = groundDrag;
        Stun();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        rb.drag = 0;
    }

    public override void DoFixedUpdateState()
    {
        base.DoFixedUpdateState();
    }

    public override void DoUpdateState()
    {
        stunDuration -= Time.deltaTime;
        if (stunDuration <= 0 && isStunned)
        {
            isStunned = false;
            UnStun();
        }
        base.DoUpdateState();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    public override void CheckTransitions()
    {
        if (isStunned)
        {
            return;
        }

        base.CheckTransitions();
    }
    private void Stun()
    {
        if (isStunned)
        {
            AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.PlayerHit, rb.transform.position);
            gameObject.GetComponentInChildren<FirstPersonCamera>().enabled = false;
            rb.velocity = Vector3.zero;
            rb.drag = groundDrag;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddTorque(Vector3.right * 8f, ForceMode.Impulse);
        }
    }

    private void UnStun()
    {
        if (!isStunned)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.rotation = Quaternion.identity;
            gameObject.GetComponentInChildren<FirstPersonCamera>().enabled = true;
        }
    }
}