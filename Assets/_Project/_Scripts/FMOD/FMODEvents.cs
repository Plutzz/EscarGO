using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: SerializeField] public EventReference jumpSound { get; private set; }


}
