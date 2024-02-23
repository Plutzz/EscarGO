using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsUI : MonoBehaviour
{
    public void AnotherShift()
    {
        // Play another run of the game
        SceneManager.LoadScene("GameScene");
    }

    public void ClockOut()
    {
        // return to lobby
        SceneManager.LoadScene("StartMenu");
    }
}
