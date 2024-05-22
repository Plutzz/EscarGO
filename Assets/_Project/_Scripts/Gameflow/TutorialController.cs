using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName;
    public void CheckTutorial()
    {
        if (PlayerPrefs.GetInt("FinishedTutorial") < 1)
        {
            StartTutorial();
        }
        else if (PlayerPrefs.GetInt("FinishedTutorial") == 1)
        {
            PlayerPrefs.SetInt("FinishedTutorial", 2);
        }
    }

    public async void StartTutorial()
    {
        // Host relay
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
        TutorialManager.currentTutorialStep = 0;
        NetworkManager.Singleton.SceneManager.LoadScene(tutorialSceneName, LoadSceneMode.Single);
    }
}
