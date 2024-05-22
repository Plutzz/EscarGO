using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private float roundTime = 180f;
    [SerializeField] private Vector3[] spawnPositions;
    private float timeLeft;
    public override void OnNetworkSpawn()
    {
        // If this is not called on the server, return
        if(!IsServer) return;

        ScoringSingleton.Instance.AssignPlayerNumbers();
        

        int index = 0;
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            teleportPlayersClientRpc(spawnPositions[index % spawnPositions.Length], new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { client.ClientId } } });
            index++;
        }
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
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();

        // Reset player at start of game
        player.timerText.gameObject.SetActive(true);
        player.scoreText.gameObject.SetActive(true);
        player.GetComponent<InputManager>().SwitchActionMap("Player");
        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
        stateMachine.ChangeState(stateMachine.IdleState);

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
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().SwitchActionMap("Player");
        Cursor.lockState = CursorLockMode.Confined;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().scoreText.text = "0 Points";
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>().ClearInventory();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InputManager>().SwitchActionMap("Player");
        AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Menu);
    }

    [ClientRpc]
    private void teleportPlayersClientRpc(Vector3 spawnPos, ClientRpcParams param = default)
    {
        Transform _player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.transform;

        Debug.Log("Teleported Player to : " + spawnPos);

        // Disables player movement
        _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _player.GetComponent<Rigidbody>().position = spawnPos;
        //_player.GetComponent<ClientNetworkTransform>().Teleport(spawnPos, Quaternion.identity, transform.localScale);

    }



}
