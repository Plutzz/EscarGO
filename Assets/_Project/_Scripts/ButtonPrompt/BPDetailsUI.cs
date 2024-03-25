using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BPDetailsUI : MonoBehaviour
{
    [SerializeField] private Image controllerButton;
    [SerializeField] private TMP_Text keyboardButton;
    [SerializeField] private TMP_Text promptText;

    public ButtonPromptDetails bpDetails;
    private InputManager inputManager;

    void Start()
    {
        // controllerButton.color = new Color(1, 1, 1, 0);
        inputManager = transform.parent.parent.parent.GetComponent<InputManager>();

        controllerButton.enabled = false;
        keyboardButton.enabled = false;
    }

    void Update()
    {
        // Have this swap between keyboard and console
        if (inputManager.playerInput != null)
        {
            if (inputManager.playerInput.currentControlScheme == "Keyboard")
            {
                controllerButton.enabled = false;
                keyboardButton.enabled = true;

                keyboardButton.text = bpDetails.keyboardPrompt;
            }
            else if (inputManager.playerInput.currentControlScheme == "Gamepad")
            {
                controllerButton.enabled = true;
                keyboardButton.enabled = false;

                controllerButton.sprite = bpDetails.buttonImgPrompt;
            }
        }
        promptText.text = bpDetails.prompt;
    }

}
