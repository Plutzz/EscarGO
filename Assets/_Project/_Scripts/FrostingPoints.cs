using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrostingPoints : MonoBehaviour
{
    public List<GameObject> points = new List<GameObject>();

    public List<Vector2> pointLocations = new List<Vector2>();

    void Start()
    {
        foreach (GameObject point in points)
        {
            pointLocations.Add(new Vector2(point.transform.position.x, point.transform.position.y));
        }
    }
}
