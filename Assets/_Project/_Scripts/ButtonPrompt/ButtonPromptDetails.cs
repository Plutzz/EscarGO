using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Button Prompt", menuName = "ButtonPrompt/Button Prompt Detail")]
public class ButtonPromptDetails : ScriptableObject
{
    public string keyboardPrompt;
    public Sprite buttonImgPrompt;
    public string prompt;
}
