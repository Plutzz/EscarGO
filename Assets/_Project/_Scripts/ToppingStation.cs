using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingStation : SuperStation
{
    [SerializeField] private GameObject toppingCircle;
    [SerializeField] private int toppingCircleAmount = 5;
    [SerializeField] private float maxX = 0.37f;
    [SerializeField] private float maxZ = 0.37f;
    private bool success = false;
    private LayerMask minigameLayer;
    private Ray ray;

    private int toppingCircleLeft;

    public override void Activate()
    {
        
    }

    public override void DeActivate()
    {
        
    }

    public override bool ActivityResult
    {
        get { return success; }
        set { success = value; }
    }

    private void Start() {
        minigameLayer = LayerMask.GetMask("Minigame");

        toppingCircleLeft = toppingCircleAmount;

        for(int i = 1; i <= toppingCircleAmount; i++)
        {
            Instantiate(toppingCircle, transform.position + new Vector3(Random.Range(-maxX, maxX), 0.54f, Random.Range(-maxZ, maxZ)), transform.rotation);
        }
    }

    private void Update() {

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
            Debug.Log("done");
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
