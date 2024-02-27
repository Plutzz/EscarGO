using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference Music { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference JumpSound { get; private set; }


}
