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
    //[field: SerializeField] public EventReference CompleteOrder { get; private set; }
    //[field: SerializeField] public EventReference ItemPickup { get; private set; }
    //[field: SerializeField] public EventReference DoorClose { get; private set; }
    //[field: SerializeField] public EventReference PlayerWalkTile { get; private set; }
    //[field: SerializeField] public EventReference PlayerWalkWood { get; private set; }
    [field: SerializeField] public EventReference[] SfxArray { get; private set; }
    public enum NetworkSFXName
    {
        CompleteOrder,
        ItemPickup,
        DoorClose,
        PlayerWalkTile,
        PlayerWalkWood,
        PlayerThrow,
        PlayerHit,
        CustomerLeave,
        CustomerEat,
        CartMove,
        DishClatter,
        TurnKnob,
        StationTicking,
        ToppingShake,
        KnifeCut,
        WafflePour,
        RollingPin,
        CustomerEnter,
        ItemTrash,
    }
}
