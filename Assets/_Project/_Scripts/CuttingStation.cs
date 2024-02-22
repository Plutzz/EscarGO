using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingStation : SuperStation
{
    private bool success = false;
    public override void Activate()
    {
    }

    public override void DeActivate()
    {
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }
}
