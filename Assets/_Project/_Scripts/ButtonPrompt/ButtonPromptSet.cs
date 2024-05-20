using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "New Button Prompt Set", menuName = "ButtonPrompt/Button Prompt Set")]
public class ButtonPromptSet : MonoBehaviour
{
    public List<ButtonPromptDetails> buttonPromptSet;
    public List<MeshRenderer> meshRenderers;

    public void ChangeOutline(Material outline)
    {
        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer != null)
            {
                Material[] materials = meshRenderer.materials;
                for(int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.StartsWith("Outlines"))
                    {
                        materials[i] = outline;
                        meshRenderer.materials = materials;
                    }

                }
                
                
            }
        }

    }
}
