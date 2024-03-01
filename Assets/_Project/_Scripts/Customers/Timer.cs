using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timerDuration = 30f; 

    public void StartTimer(Customer customer)
    {
        StartCoroutine(TimerCoroutine(customer));
    }

    private IEnumerator TimerCoroutine(Customer customer)
    {
        yield return new WaitForSeconds(timerDuration);
        customer.Leave();
    }
}
