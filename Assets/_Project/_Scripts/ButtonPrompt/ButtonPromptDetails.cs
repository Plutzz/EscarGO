using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Button Prompt", menuName = "ButtonPrompt/Button Prompt Detail")]
public class ButtonPromptDetails : ScriptableObject
{
    public Sprite keyboardImgPrompt;
    public Sprite controllerImgPrompt;
    public string buttonPrompts;
}
