using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    # region Object References
    [Header("Object References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight;
    [SerializeField] private Transform orientation;
    [SerializeField] private TextMeshPro nameTag;
    [SerializeField] private GameObject graphics;

    private PlayerStateMachine stateMachine;
    private PlayerState currentState;
    private Rigidbody rb;
    private InputManager inputManager;

    [Header("Movement Variables")]
    [SerializeField] private float sprintSpeed = 5f;
    private float moveSpeed;
    private bool sprinting = false;
    public bool canSprint = true;
    #endregion

    [SerializeField] private SettingsMenu pauseMenu;
    [SerializeField] private FirstPersonCamera cameraScript;
    [SerializeField] private Vector3 spawnPos;

    [Header("Spectate Variables")]
    [SerializeField] private Vector3 localCameraPosition;
    [SerializeField] private List<GameObject> toggledGameObjects;
    [SerializeField] private List<MonoBehaviour> toggledMonoBehaviours;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            enabled = false;
            return;
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            SetupPlayerName();
        }

        stateMachine = GetComponent<PlayerStateMachine>();
        moveSpeed = stateMachine.moveSpeed;
        rb = stateMachine.rb;
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        rb.velocity = Vector3.zero;
        rb.position = spawnPos;
    }
    private async void SetupPlayerName()
    {
        await AuthenticationService.Instance.GetPlayerNameAsync();

        string _playerName = AuthenticationService.Instance.PlayerName;

        gameObject.name = _playerName;
        nameTag.text = _playerName;
    }

    void Update()
    {
        sprinting = stateMachine.inputManager.SprintIsPressed;

        currentState = stateMachine.currentState;

        if (currentState == stateMachine.AirborneState) return;
        

        if (sprinting && canSprint)
        {
            stateMachine.moveSpeed = sprintSpeed;
        }
        else
        {
            stateMachine.moveSpeed = moveSpeed;
        }

        if(inputManager.PausePressedThisFrame && pauseMenu != null)
        {
            pauseMenu.OpenMenu();
        }

        if(orientation != null && cameraScript != null)
            orientation.eulerAngles = new Vector3 (0f, cameraScript.transform.eulerAngles.y, 0f);

    }
}
