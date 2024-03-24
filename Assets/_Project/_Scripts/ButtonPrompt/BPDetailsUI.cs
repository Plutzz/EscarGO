using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BPDetailsUI : MonoBehaviour
{
    [SerializeField] private Image promptImage;
    [SerializeField] private TMP_Text promptText;

    public ButtonPromptDetails bpDetails;

    void FixedUpdate()
    {
        // Have this swap between keyboard and console
        promptImage.sprite = bpDetails.keyboardImgPrompt;
        promptText.text = bpDetails.buttonPrompts;
    }

}
