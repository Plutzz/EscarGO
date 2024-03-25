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
        inputManager = transform.parent.parent.parent.GetComponent<InputManager>();

        controllerButton.gameObject.SetActive(false);
        keyboardButton.gameObject.SetActive(false);
    }

    void Update()
    {
        // Have this swap between keyboard and console
        if (inputManager.playerInput != null)
        {
            if (inputManager.playerInput.currentControlScheme == "Keyboard")
            {
                keyboardButton.text = bpDetails.keyboardPrompt;

                controllerButton.gameObject.SetActive(false);
                keyboardButton.gameObject.SetActive(true);
            }
            else if (inputManager.playerInput.currentControlScheme == "Gamepad")
            {
                controllerButton.sprite = bpDetails.buttonImgPrompt;

                keyboardButton.gameObject.SetActive(false);
                controllerButton.gameObject.SetActive(true);
            }
        }
        promptText.text = bpDetails.prompt;
    }

}
