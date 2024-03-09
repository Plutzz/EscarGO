using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Criteria", menuName = "Criteria", order = 0)]
public class Criteria : ScriptableObject {

    public List<Required> objectPairs = new List<Required>();
    public void ResetHave()
    {
        foreach(Required items in objectPairs)
        {
            items.resetHave();
        }
    }

    [System.Serializable]
    public class Required
    {

        public Item item;
        public int need;
        private int have = 0;

        public void turnIn()
        {
            have += 1;
        }

        public int getHave()
        {
            return have;
        }

        public void resetHave()
        {
            have = 0;
        }
    }
}