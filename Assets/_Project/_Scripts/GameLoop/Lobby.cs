using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    

    public void StartMatch()
    {
        SceneManager.LoadScene("GameScene");
    }
}
