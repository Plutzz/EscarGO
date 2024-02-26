using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SuperStation : MonoBehaviour
{
    public abstract bool ActivityResult { get; set; }

    public abstract GameObject VirtualCamera { get; set; }

    public abstract void Activate();

    public abstract void DeActivate();

}
