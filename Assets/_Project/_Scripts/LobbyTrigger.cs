using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyTrigger : NetworkBehaviour
{
    [SerializeField] private string SceneName;
    private NetworkVariable<int> numPlayersInCollider = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            numPlayersInCollider.Value += 1;

            if (numPlayersInCollider.Value >= 1)
            {
                Debug.Log("Loading Next Scene");
                NetworkManager.Singleton.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IsOwner) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            numPlayersInCollider.Value -= 1;
        }
    }



}
