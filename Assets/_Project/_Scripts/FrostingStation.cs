using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostingStation : Interactable
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

    public List<Vector2> patternPoints; // Define the desired pattern points in the Inspector
    public float matchThreshold = 0.1f; // Adjust the match threshold based on your requirements

    private List<Vector2> userPoints = new List<Vector2>();
    private bool isTracing = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartTracing();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueTracing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopTracing();
        }
    }

    void StartTracing()
    {
        isTracing = true;
        userPoints.Clear();
    }

    void ContinueTracing()
    {
        if (isTracing)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            userPoints.Add(mousePosition);
            DrawLine();
        }
    }

    void StopTracing()
    {
        if (isTracing)
        {
            isTracing = false;
            CheckPattern();
        }
    }

    void DrawLine()
    {
        for (int i = 0; i < userPoints.Count - 1; i++)
        {
            Debug.DrawLine(userPoints[i], userPoints[i + 1], Color.red);
        }
    }

    void CheckPattern()
    {
        if (IsPatternMatched())
        {
            Debug.Log("Pattern matched!");
        }
        else
        {
            Debug.Log("Pattern not matched. Try again.");
        }
    }

    bool IsPatternMatched()
    {
        if (userPoints.Count != patternPoints.Count)
        {
            return false;
        }

        for (int i = 0; i < userPoints.Count; i++)
        {
            if (Vector2.Distance(userPoints[i], patternPoints[i]) > matchThreshold)
            {
                return false;
            }
        }

        return true;
    }
}
