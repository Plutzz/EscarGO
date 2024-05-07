using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.Events;

public abstract class SuperStation : NetworkBehaviour
{
    protected Item resultingItem;
    protected bool inUse = false;
    [SerializeField] protected UnityEvent finishEvents;
    public abstract bool StationInUse { get; }
    public abstract bool ActivityResult { get; set; }

    public abstract CinemachineVirtualCamera VirtualCamera { get; set; }

    public abstract void Activate(CraftableItem successfulItem);
    public abstract void GetItem();

    // This should only be called on the client that is currently in the station
    public abstract void DeActivate();

}
