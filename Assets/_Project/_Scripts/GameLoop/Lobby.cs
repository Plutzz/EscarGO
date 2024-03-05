using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    private string playScene = "David Recipe"; //temporary until actual game scene
    public void StartMatch()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(playScene);
    }
}
