using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Custom input handler class to be able to have rebindable inputs
public class InputManager : Singleton<InputManager>
{
    // Move Input
    public Vector2 MoveInput {  get; private set; }

    // Interact input
    public bool InteractPressedThisFrame {  get; private set; }
    public bool InteractReleasedThisFrame { get; private set; }
    public bool InteractIsPressed { get; private set; }


    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction interactAction;

    protected override void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
    }

    private void UpdateInputs()
    {
        // Move Input Variables
        MoveInput = moveAction.ReadValue<Vector2>();

        // Interact Input Variables
        InteractPressedThisFrame = interactAction.WasPressedThisFrame();
        InteractIsPressed = interactAction.IsPressed();
        InteractReleasedThisFrame = interactAction.WasReleasedThisFrame();
    }
}
