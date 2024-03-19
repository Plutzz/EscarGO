using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StrikeUI : NetworkBehaviour
{
    public void RemoveStar()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
