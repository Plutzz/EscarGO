using DG.Tweening;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using FMODUnity;

public class Customer : NetworkBehaviour
{

    [Header("Orders")]
    private Criteria criteria;
    [SerializeField] private GameObject orderPrefab;
    [SerializeField] private GameObject orderObject;
    [SerializeField] private Vector3 orderOffset = new Vector3(0f, 1f, 0f);
    private Material orderMaterial;

    [Header("Timer")]
    private float timer;
    private bool timerActive = false;
    [SerializeField] private float patienceTime = 30f; // Time in seconds until customer leaves

    [Header("Seating Variables")]
    public int assignedPlayer;
    public bool orderReceived;
    public bool eating;
    private Chair currentChair;

    //[Header("UI")]
    //[SerializeField] private TextMeshProUGUI scoreTextPrefab; 
    //private TextMeshProUGUI scoreTextInstance; 

    [SerializeField] private GameObject scoreText3DPrefab; 
    private GameObject scoreText3DInstance;

    private Animator animator;

    private StudioEventEmitter eatSFX;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        timer = patienceTime;

        // Instantiate a new GameObject for the order sprite
        orderObject = Instantiate(orderPrefab, transform);

        // Position the order sprite above the customer
        orderObject.transform.localPosition = orderOffset;

        // Add a SpriteRenderer component to the order GameObject
        orderMaterial = Instantiate(orderObject.GetComponent<Renderer>().material);
        orderObject.GetComponent<Renderer>().material = orderMaterial;

        orderObject.SetActive(false);

        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Seated", false);

        if (!IsServer) return;


    }

    void Update()
    {
        // Called on every client
        TimerHandler();

        if(!IsServer) return;

        if (timer <= 0 && currentChair != null)
        {
            LeaveServerRpc(false);
        }
    }

    [ClientRpc]
    public void ActivateTimerClientRpc(bool isActive)
    {
        timerActive = isActive;
    }

    private void TimerHandler()
    {
        // Handle the timer
        if (timerActive)
        {
            timer -= Time.deltaTime;
            UpdateTimerCircle();
        }
    }

    [ClientRpc]
    public void GetCustomerOrderClientRpc(ClientRpcParams sendParams)
    {
        GetCustomerOrder(true);
    }

    public void GetCustomerOrder(bool showItem)
    {
        orderObject.SetActive(showItem);
        // Set the order sprite to the item's sprite
        criteria = Instantiate(CustomerSpawner.Instance.GetCriteria());

        if (criteria != null && criteria.objectPairs[0].item.itemSprite != null)
        {
            orderMaterial.SetTexture("_Texture", criteria.objectPairs[0].item.itemSprite.texture);
        }
        else
        {
            Debug.LogWarning("Order item or its sprite is not assigned!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveServerRpc(bool gotOrder)
    {
        LeaveClientRpc();

        if (eating) return;
        
        if (gotOrder)
        {
            eating = true;
            Debug.Log("Singleton Instance " + ScoringSingleton.Instance);
            Debug.Log("assignedPlayer " + assignedPlayer);
            Debug.Log("critera " + criteria);
            Debug.Log("score " + criteria.score);
            ScoringSingleton.Instance.AddScoreServerRpc(assignedPlayer, criteria.score);
            ActivateTimerClientRpc(false);

            // Timer for customer eating
            StartCoroutine(FufillOrderWait(10));

            DisplayPointsEarned(criteria.score);
        }
        else
        {
            CustomerSpawner.Instance.customerCount--;
            // Substract points instead
            //ScoringSingleton.Instance.RecieveStrikeServerRpc(assignedPlayer);
            Exit();
        }
    }
    private IEnumerator FufillOrderWait(float seconds)
    {
        PlayEatSfxEmitterClientRpc(FMODEvents.NetworkSFXName.CustomerEat, gameObject, true);
        yield return new WaitForSeconds(seconds);
        PlayEatSfxEmitterClientRpc(FMODEvents.NetworkSFXName.CustomerEat, gameObject, false);
        Destroy(GetComponent<StudioEventEmitter>());
        CustomerSpawner.Instance.customerCount--;
        Exit();
    }


    [ClientRpc]
    public void LeaveClientRpc()
    {
        DOTween.To(() => orderMaterial.GetFloat("_Radius"), x => orderMaterial.SetFloat("_Radius", x), 0, 2).SetEase(Ease.InOutExpo);
        DOTween.To(() => orderMaterial.GetFloat("_Border_Thickness"), x => orderMaterial.SetFloat("_Border_Thickness", x), 1, 1).SetEase(Ease.InOutSine);
        orderMaterial.SetFloat("_Spacing_Width", 0);
    }

    private void UpdateTimerCircle()
    {
        orderMaterial.SetFloat("_Fill_Amount", Mathf.InverseLerp(0, patienceTime, timer));
    }

    public void Exit()
    {
        AudioManager.Instance.PlayOneShotAllServerRpc(FMODEvents.NetworkSFXName.CustomerLeave, transform.position);
        animator.SetBool("Seated", false);

        Debug.Log("Thank you!");
        if (currentChair != null)
        {
            currentChair.RemoveCustomer();
            currentChair = null; 
        }
        GetComponent<CustomerMovement>().MoveToExit();
    }

    public bool TryCompleteOrder(PlayerInventory inventory)
    {
        if (inventory.CurrentlyHasItem())
        {

            foreach (Criteria.Required criteriaItem in criteria.objectPairs)
            {

                if (inventory.getCurrentItem().itemName == criteriaItem.item.itemName)
                {
                    inventory.TurnInActiveItems();
                    criteriaItem.turnIn();
                    Debug.Log("Turned in " + criteriaItem.getHave() + " " + criteriaItem.item.itemName);

                    if (FulfilledAllCriteria())
                    {
                        LeaveServerRpc(true);
                    }
                }
            }
            return true;
        }
        else
        {
            TipsManager.Instance.SetTip("Incorrect Order", 2f);
            return false;
        }
    }
    private bool FulfilledAllCriteria()
    {
        foreach (Criteria.Required criteriaItem in criteria.objectPairs)
        {
            if (criteriaItem.getHave() != criteriaItem.need)
            {
                return false;
            }
        }

        return true;
    }
    public void EnterChair(Chair chair)
    {
        animator.SetBool("Seated", true);

        currentChair = chair;
    }

    private void DisplayPointsEarned(int pointsEarned)
    {
        scoreText3DInstance = Instantiate(scoreText3DPrefab, orderObject.transform.position, Quaternion.identity);
        scoreText3DInstance.transform.SetParent(orderObject.transform);
        scoreText3DInstance.transform.localPosition = Vector3.up;
        scoreText3DInstance.GetComponent<TextMeshPro>().text = "+" + pointsEarned.ToString();
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
    private void PlayEatSfxEmitterClientRpc(FMODEvents.NetworkSFXName sound, NetworkObjectReference gameObj, bool play)
    {
        if(play)
        {
            eatSFX = AudioManager.Instance.InitializeEventEmitter(sound, gameObj);
            eatSFX.Play();
        }
        else
        {
            eatSFX?.Stop();
        }
    }
}


