using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyTrigger : NetworkBehaviour
{
    [SerializeField] private string SceneName;
    private int numPlayersInCollider = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            numPlayersInCollider++;

            if (numPlayersInCollider >= 1)
            {
                Debug.Log("Loading Next Scene");
                NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            numPlayersInCollider -= 1;
        }
    }



}
