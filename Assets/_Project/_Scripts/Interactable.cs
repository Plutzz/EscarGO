using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract bool ActivityResult { get; set; }

    public abstract void Activate();

    public abstract void DeActivate();

}
