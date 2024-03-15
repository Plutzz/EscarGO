using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName ="Recipe")]
public class Recipe : ScriptableObject
{
    public string[] ingredients; 
    public new string name;
}
