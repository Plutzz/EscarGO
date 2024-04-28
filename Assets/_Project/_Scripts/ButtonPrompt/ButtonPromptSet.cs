using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "New Button Prompt Set", menuName = "ButtonPrompt/Button Prompt Set")]
public class ButtonPromptSet : MonoBehaviour
{
    public List<ButtonPromptDetails> buttonPromptSet;
    public MeshRenderer meshRenderer;

    public void ChangeOutline(Material outline)
    {
        if (meshRenderer != null)
        {
            Material[] materials = meshRenderer.materials;
            materials[1] = outline;
            meshRenderer.materials = materials;
        }
    }
}
