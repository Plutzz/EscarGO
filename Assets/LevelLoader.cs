using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;
    // public float transitionTime = 1f;
    
    // private IEnumerator SceneTransition()
    // {

    //     transition.SetTrigger("Start");

    //     yield return new WaitForSeconds(transitionTime);
    // }

    public void fadeTransition()
    {
        transition.SetTrigger("Start");
    }
}
