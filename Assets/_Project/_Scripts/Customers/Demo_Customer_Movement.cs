using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Demo_Customer_Movement : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        DrawPath();
        
    }

    public void SetDestination(Vector3 destination) { 
        agent.destination = destination;
    }


    public void SetAgentActive(bool state) { 
        agent.enabled = state;
    }

    private void DrawPath() {
        if (agent != null && agent.hasPath)
        {
            Vector3[] corners = agent.path.corners;

            // Draw lines between each corner of the path
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.red);
            }
        }
    }
}
