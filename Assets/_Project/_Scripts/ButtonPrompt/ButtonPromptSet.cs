using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Button Prompt Set", menuName = "ButtonPrompt/Button Prompt Set")]
public class ButtonPromptSet : ScriptableObject
{
    public List<ButtonPromptDetails> buttonPromptSet;
}
