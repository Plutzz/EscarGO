using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialPlayerDetectorr : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private int tutorialStep = 2;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(tutorialManager.getCurrentStep());
            if(tutorialManager.getCurrentStep() == tutorialStep)
            {
                tutorialManager.FinishedTutorialStep(tutorialStep);
                Destroy(this.gameObject);
            }
        }
    }
}