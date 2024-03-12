using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Customer_Seating : Singleton<Demo_Customer_Seating>
{
    //[SerializeField] private Demo_Chair[] allChairs;

    private Queue<Demo_Chair> availableChairs = new Queue<Demo_Chair>();

    public int AvailableChairCount() {
        return availableChairs.Count;
    }
    public Demo_Chair GetAvailableChair() { 
        return availableChairs.Dequeue();
    }

    public void ChairAvailable(Demo_Chair chair) {
        availableChairs.Enqueue(chair);
    }

}
