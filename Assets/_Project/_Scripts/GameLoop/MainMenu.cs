using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject lobbyHud;


    private void Start()
    {
        Application.targetFrameRate = 144;
    }

    public void OpenLobbyMenu()
    {
        lobbyMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void CloseLobbyMenu()
    {
        lobbyMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void PlayButtonClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.ButtonClick, Vector3.zero);
    }

    public void PlayButtonHover()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.ButtonHover, Vector3.zero);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
