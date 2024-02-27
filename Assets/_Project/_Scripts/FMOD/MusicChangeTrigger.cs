using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private AudioManager.MusicArea area;

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<Collider>().tag.Equals("Player"))
        {
            AudioManager.Instance.SetMusicArea(area);
        }
    }
}