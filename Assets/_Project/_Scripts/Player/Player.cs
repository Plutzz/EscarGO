using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] public GameObject crosshair;

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

    [Header("SFX")]
    private StudioEventEmitter walkSFX;

    public override void OnNetworkSpawn()
    {

        if(!IsOwner)
        {
            enabled = false;
            return;
        }
        AudioManager.Instance.SetMusicArea(AudioManager.MusicArea.Lobby);
        Camera.main.GetComponent<StudioListener>().attenuationObject = gameObject;
        ClientConnectedServerRpc(NetworkManager.Singleton.LocalClientId);
        nameTag.enabled = false;
        stateMachine = GetComponent<PlayerStateMachine>();
        moveSpeed = stateMachine.moveSpeed;
        rb = stateMachine.rb;
        inputManager = GetComponent<InputManager>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene prevScene, Scene nextScene)
    {
        Camera.main.GetComponent<StudioListener>().attenuationObject = gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientConnectedServerRpc(ulong clientId)
    {
        foreach (var player in NetworkManager.Singleton.ConnectedClients)
        {
            player.Value.PlayerObject.GetComponent<Player>().SetupNametagClientRpc(null, new ClientRpcParams {  Send = new ClientRpcSendParams {  TargetClientIds = new List<ulong> { clientId } } });
        }
    }

    [ClientRpc]
    public void SetupNametagClientRpc(string username, ClientRpcParams rpcParams = default)
    {
        SetupName(username);
    }

    public void SetupName(string username)
    {
        if (username == null)
        {
            username = AuthenticationService.Instance.PlayerName.Substring(0, AuthenticationService.Instance.PlayerName.Length - 5);
        }
        nameTag.text = username;
        gameObject.name = username;
    }
    private void Start()
    {
        rb.velocity = Vector3.zero;
        rb.position = spawnPos;
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

    /// <summary>
    /// Play an emitter on all clients from this script
    /// EMITTERS MUST BE INITIALIZED AND PLAYED BEFORE TRYING TO STOP THEM
    /// MUST BE CALLED FROM THE SERVER
    /// To play sound: play = true
    /// To stop sound: play = false
    /// <param name="sound"></param>
    /// <param name="gameObj"></param>
    /// <param name="play">/param>
    /// </summary>

    [ClientRpc]
    private void PlayWalkSfxEmitterClientRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        if (play)
        {
            walkSFX = AudioManager.Instance.InitializeEventEmitter(sound, gameObj);
            walkSFX.Play();
        }
        else
        {
            walkSFX?.Stop();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayWalkSfxEmitterServerRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        PlayWalkSfxEmitterClientRpc(sound, gameObj, play);
    }
}
