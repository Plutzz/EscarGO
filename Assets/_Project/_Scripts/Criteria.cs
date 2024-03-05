using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Criteria", menuName = "Criteria", order = 0)]
public class Criteria : ScriptableObject {

    public List<Required> objectPairs = new List<Required>();

    [System.Serializable]
    public class Required
    {
        public CraftableItem item;
        public int amount;
    }
}