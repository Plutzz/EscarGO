using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settingMenu;

    public void LoadLobby()
    {
        // Go Lobby here
        SceneManager.LoadScene("Lobby");
    }
}