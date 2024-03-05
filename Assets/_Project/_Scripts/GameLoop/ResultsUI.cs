using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsUI : MonoBehaviour
{
    private string playScene = "David Recipe"; //temporary until actual game scene

    public void GameComplete()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void AnotherShift()
    {
        // Play another run of the game
        SceneManager.LoadScene(playScene);
    }

    public void ClockOut()
    {
        // return to lobby
        SceneManager.LoadScene("StartMenu");
    }
}
