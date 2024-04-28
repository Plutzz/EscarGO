using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private float roundTime = 180f;
    private float timeLeft;
    public override void OnNetworkSpawn()
    {
        // If this is not called on the server, return
        if(!IsServer) return;

        ScoringSingleton.Instance.AssignPlayerNumbers();
        teleportPlayersClientRpc();
        StartGameClientRpc();
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if (!IsServer) return;

        if(timeLeft <= 0)
        {
            EndGameServerRpc();
        }
    }
    public string GetTime()
    {
        int secondsLeft = Mathf.RoundToInt(timeLeft);
        int minutesLeft = secondsLeft / 60;
        secondsLeft = secondsLeft % 60;
        return minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        Cursor.lockState = CursorLockMode.Locked;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().timerText.gameObject.SetActive(true);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().scoreText.gameObject.SetActive(true);
        timeLeft = roundTime;
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRpc()
    {
        EndGameClientRpc();
        // Disable all player objects
        //foreach (var _player in NetworkManager.Singleton.ConnectedClientsList)
        //{
        //    _player.PlayerObject.gameObject.SetActive(false);
        //}
        NetworkManager.Singleton.SceneManager.LoadScene("NetworkResults", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        Cursor.lockState = CursorLockMode.Confined;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().scoreText.text = "0 Points";
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>().ClearInventory();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().SwitchActionMap("Player");
        AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Menu);
    }

    [ClientRpc]
    private void teleportPlayersClientRpc()
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.transform;

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _player.GetComponent<Rigidbody>().position = spawnPos;
        //_player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);

    }



}
