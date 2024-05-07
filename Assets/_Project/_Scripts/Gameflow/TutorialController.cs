using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName;
    private void Awake()
    {
        if (PlayerPrefs.GetInt("FinishedTutorial") < 1)
        {
            TutorialManager.currentTutorialStep = 0;
            SceneManager.LoadScene(tutorialSceneName);
        }
        else if (PlayerPrefs.GetInt("FinishedTutorial") == 1)
        {
            PlayerPrefs.SetInt("FinishedTutorial", 2);
        }
    }
}
