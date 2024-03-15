using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Criteria", menuName = "Criteria", order = 0)]
[System.Serializable]
public class CustomerOrder : ScriptableObject
{
    public Item item;

    //public void turnin()
    //{
    //    have += 1;
    //}

    //public int gethave()
    //{
    //    return have;
    //}

    //public void resethave()
    //{
    //    have = 0;
    //}
}
