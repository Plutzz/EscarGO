using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timerDuration = 60f; // Default timer duration

    public void StartTimer(Customer customer)
    {
        StartCoroutine(TimerCoroutine(customer));
    }

    private IEnumerator TimerCoroutine(Customer customer)
    {
        yield return new WaitForSeconds(timerDuration);
        customer.Leave();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
