using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class ResultsUI : MonoBehaviour
{
    public void GameComplete()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("NetworkResults", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void AnotherShift()
    {
        // Ready up for another run of the game
        ResultsManager.Instance.PlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void ClockOut()
    {
        // If any player presses this, bring them back to the lobby
        // NetworkManager.Singleton.SceneManager.LoadScene("BenLobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    
        LobbyAPI.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton?.gameObject);
        Destroy(AudioManager.Instance?.gameObject);
        Destroy(gameObject);
        SceneManager.LoadSceneAsync("BenLobby");
    }


}
