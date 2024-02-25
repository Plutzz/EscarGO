using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Should handle any logic in terms of how the game is ran
public class MatchManager : MonoBehaviour
{
    [SerializeField] private GameObject debuggingUI;
    [SerializeField] private GameObject resultsUI;

    void Awake()
    {
        debuggingUI.SetActive(true);
        resultsUI.SetActive(false);
    }

    public void FinishGame()
    {
        debuggingUI.SetActive(false);
        resultsUI.SetActive(true);
    }
}
