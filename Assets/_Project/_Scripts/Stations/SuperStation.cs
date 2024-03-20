using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public abstract class SuperStation : MonoBehaviour
{
    protected Item resultingItem;
    public abstract bool ActivityResult { get; set; }

    public abstract CinemachineVirtualCamera VirtualCamera { get; set; }

    public abstract void Activate(Item successfulItem);

    // This should only be called on the client that is currently in the station
    public abstract void DeActivate();

}
