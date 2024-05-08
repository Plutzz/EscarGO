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

public class TutorialManager : Singleton<TutorialManager>
{
    public static int currentTutorialStep;
    public static TutorialManager TutorialInstance { get; private set; }

    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [Header("Tutorial UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private GameObject tutorialTextObject;
    [SerializeField] private float tutorialTextShowTime;
    [SerializeField] private float letterSpeed = .1f;

    private Coroutine currentCoroutine;
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

    public void FinishedTutorialStep(int finishedStepIndex)
    {
        Debug.Log("Finished tutorial step: " + finishedStepIndex);


        if (finishedStepIndex >= tutorialSteps.Count-1)
        {
            PlayerPrefs.SetInt("FinishedTutorial", 1);
            SceneManager.LoadScene(0);
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


        yield return new WaitForSeconds(tutorialTextShowTime);

        tutorialTextObject.SetActive(false);

        if (currentStep.autoplayNextStep) {
            if (currentStep.timeToNextStep > 0) {
                yield return new WaitForSeconds(currentStep.timeToNextStep);
            }

            FinishedTutorialStep(currentTutorialStep);
        }
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
