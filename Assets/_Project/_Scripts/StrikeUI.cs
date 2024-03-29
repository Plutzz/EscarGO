using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StrikeUI : NetworkBehaviour
{
    [SerializeField] private GameObject starPrefab;
    public void RemoveStar()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

    public void ResetStars(int starCount)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < starCount; i++)
        {
            Instantiate(starPrefab, transform);
        }
    }
}
