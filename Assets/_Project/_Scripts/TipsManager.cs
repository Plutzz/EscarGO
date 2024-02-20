using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsManager : MonoBehaviour
{
    public static TipsManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI textBox;

    private float textAppearTime;
    private float timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        SetTip("Left-Click on stations", 5);
    }

    private void Update()
    {
        if (timer < textAppearTime) {
            timer += Time.deltaTime;
            if (timer > textAppearTime)
            {
                textBox.text = "";
                textAppearTime = 0;
                
            }
        }
    }

    public void SetTip(string tipText, float appearTime) { 
        textBox.text = tipText;
        textAppearTime = appearTime;
        timer = 0;
    }
}
