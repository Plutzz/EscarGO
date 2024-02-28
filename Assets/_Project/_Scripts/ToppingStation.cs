using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ToppingStation : SuperStation
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject toppingCircle;
    [SerializeField] private int toppingCircleAmount = 5;
    [SerializeField] private float maxX = 0.37f;
    [SerializeField] private float maxZ = 0.37f;
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    private bool isTopping = false;
    private int toppingCircleLeft;
    private List<GameObject> toppingCircleObjects;

    public override void Activate()
    {
        virtualCamera.enabled = true;
        minigameLayer = LayerMask.GetMask("Minigame");

        isTopping = true;

        toppingCircleLeft = toppingCircleAmount;

        for(int i = 1; i <= toppingCircleAmount; i++)
        {
            toppingCircleObjects.Add(Instantiate(toppingCircle, transform.position + new Vector3(Random.Range(-maxX, maxX), 0.54f, Random.Range(-maxZ, maxZ)), transform.rotation));
        }
    }

    public override void DeActivate()
    {

        isTopping = false;
        virtualCamera.enabled = false;
        InputManager.Instance.playerInput.SwitchCurrentActionMap("Player");

        foreach (GameObject obj in toppingCircleObjects)
        {
            Destroy(obj);
        }
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    public override CinemachineVirtualCamera VirtualCamera
    {
        get { return virtualCamera; }
        set { virtualCamera = value; }
    }

    private void Start() {
        toppingCircleObjects = new List<GameObject>();
    }

    private void Update() {
        if(isTopping)
        {

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                if(HitToppingCircle())
                {
                    toppingCircleLeft -= 1;
                    Debug.Log(toppingCircleLeft);
                }
            }

            if(toppingCircleLeft == 0)
            {
                success = true;
            }
        }
    }

    private bool HitToppingCircle()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 900f, minigameLayer))
        {
            if(hit.collider.CompareTag("ToppingCircle"))
            {
                Destroy(hit.collider.gameObject);
                return true;
            }
        }
        
        return false;
    }
}
