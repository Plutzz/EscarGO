using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Unity.VisualScripting;
using Unity.Netcode;

public class TutorialManager : Singleton<TutorialManager>
{
    public static int currentTutorialStep;
    public static TutorialManager TutorialInstance { get; private set; }

    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [Header("Tutorial UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private GameObject tutorialTextObject;
    //[SerializeField] private float tutorialTextShowTime;
    [SerializeField] private float letterSpeed = .1f;
    [SerializeField] private List<SuperStation> stations = new List<SuperStation>();
    private Coroutine currentCoroutine;

    private int playerInventoryCount;
    private bool ready = false;
    private PlayerInventory playerInventory;
    private Customer customer;
    private Rigidbody playerRB;
    protected override void Awake()
    {
        base.Awake();

        TutorialInstance = this;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(ShowCurrentMessage());
    }

    public void Start()
    {
        playerInventory = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInventory>();
        StartCoroutine(waitForloading()); //need better way so that it only detects once everything has loaded or else passes step when player doesnt even move
    }

    private void Update()
    {
        if(playerInventoryCount < playerInventory.inventoryCount())
        {
            playerInventoryCount = playerInventory.inventoryCount();
        }

        switch (currentTutorialStep)
        {
            case 0:
                if(ready)
                {
                    if (playerRB.velocity != Vector3.zero)
                    {
                        Debug.Log("moving.......");
                        FinishedTutorialStep(0);
                    }
                }
                break;

            case 3:
                if(Input.GetMouseButtonDown(1))
                {
                    FinishedTutorialStep(3);
                    GetComponent<Canvas>().sortingOrder = 1;
                }
                break;
            case 4:
                if (Input.GetMouseButtonDown(1))
                {
                    FinishedTutorialStep(4);
                    GetComponent<Canvas>().sortingOrder = -1;
                }
                break;
            case 5:
                if (playerInventory.inventoryCount() > 0)
                {
                    FinishedTutorialStep(5);
                }
                break;
            case 6:
                if (playerInventory.inventoryCount() < playerInventoryCount)
                {
                    FinishedTutorialStep(6);
                }
                break;
            case 7:
                foreach (SuperStation station in stations)
                {
                    Debug.Log("stationing checking");
                    if (station.StationInUse == true)
                    {
                        FinishedTutorialStep(7);
                    }
                }
                break;
            case 9:
                if (customer.eating == true)
                {
                    FinishedTutorialStep(9);
                }
                break;
            default:
                break;
        }
    }

    public void FinishedTutorialStep(int finishedStepIndex)
    {
        Debug.Log("Finished tutorial step: " + finishedStepIndex);

        //end of tutorial
        if (finishedStepIndex >= tutorialSteps.Count - 1)
        {
            PlayerPrefs.SetInt("FinishedTutorial", 1);
            SceneManager.LoadScene(0);

            Cursor.lockState = CursorLockMode.None;
            
            Debug.Log("Left the lobby successfully.");
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton?.gameObject);
            Destroy(AudioManager.Instance?.gameObject);
            Destroy(gameObject);
            SceneManager.LoadSceneAsync("BenLobby");

            return;
        }

        ///Different ways to handle this
        ///1. Like this where the tutorial force each step to happen sequentially and only once
        ///2. Make sure it is greater or equal than, thus allowing the player to skip steps 
        ///3. Could also trigger previous messages if we want that
        ///I'd recommend 1 or 2 to start and then playing around with it
        if (finishedStepIndex != currentTutorialStep) {
            Debug.Log("<color=yello> Step out of order (may be intentional)</color>");
            return;
        }
        currentTutorialStep = finishedStepIndex + 1;
        ///


        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShowCurrentMessage());
    }




    IEnumerator ShowCurrentMessage()
    {
        tutorialTextObject.SetActive(true);
        TutorialStep currentStep = tutorialSteps[currentTutorialStep];


        currentStep.extraActions?.Invoke();
        if (currentStep.image == null)
        {
            tutorialImage.gameObject.SetActive(false);
        }
        else {
            tutorialImage.gameObject.SetActive(true);
            tutorialImage.sprite = currentStep.image;
        }

        tutorialText.text = "";
        foreach (char c in currentStep.tutorialMessage.ToCharArray())
        {
            tutorialText.text += c;
            if (!Input.GetMouseButton(1))
            {
                yield return new WaitForSecondsRealtime(letterSpeed);
            }
            else
            {
                Debug.Log("Speed up");
                yield return new WaitForSecondsRealtime(.005f);
            }
        }
         
        tutorialText.text = currentStep.tutorialMessage;


        if (currentStep.autoplayNextStep)
        {
            if (currentStep.timeToNextStep > 0)
            {
                yield return new WaitForSeconds(currentStep.timeToNextStep);
            }

            FinishedTutorialStep(currentTutorialStep);
            yield return null;
        }

        //yield return new WaitForSeconds(tutorialTextShowTime);

        //tutorialTextObject.SetActive(false);

        
    }
    public void SpawnCustomer()
    {
        customer = CustomerSpawner.Instance.SpawnTutorialCustomer();
        customer.setTimer(9999f);
    }

    public int getCurrentStep()
    {
        return currentTutorialStep;
    }

    IEnumerator waitForloading()
    {
        yield return new WaitForSeconds(6);

        playerRB = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>();
        ready = true;
    }
}




[Serializable]
public class TutorialStep
{
    public Sprite image;
    public string tutorialMessage;
    public UnityEvent extraActions;

    public bool autoplayNextStep;
    [Tooltip("Only for autoplaying")] public float timeToNextStep;
}
