using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<Collider>().tag.Equals("Player"))
        {
            AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Menu);
        }
    }
}