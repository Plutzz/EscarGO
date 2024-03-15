using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public abstract class SuperStation : MonoBehaviour
{
    public abstract bool ActivityResult { get; set; }

    public abstract CinemachineVirtualCamera VirtualCamera { get; set; }

    public abstract void Activate();

    // This should only be called on the client that is currently in the station
    public abstract void DeActivate();

}
