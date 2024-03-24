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

    void Start()
    {
        promptImage.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        // Have this swap between keyboard and console
        promptImage.sprite = bpDetails.keyboardImgPrompt;
        promptImage.color = new Color(1, 1, 1, 1);
        promptText.text = bpDetails.buttonPrompts;
    }

}
