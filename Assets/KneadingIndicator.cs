using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KneadingIndicator : MonoBehaviour
{
    [SerializeField] private GameObject w;
    [SerializeField] private GameObject a;
    [SerializeField] private GameObject s;
    [SerializeField] private GameObject d;

    public float alphaValue = 0.1f;

    private SpriteRenderer wColor;
    private SpriteRenderer aColor;
    private SpriteRenderer sColor;
    private SpriteRenderer dColor;

    private void Awake() {
        wColor = w.GetComponent<SpriteRenderer>();
        aColor = a.GetComponent<SpriteRenderer>();
        sColor = s.GetComponent<SpriteRenderer>();
        dColor = d.GetComponent<SpriteRenderer>();
    }

    public void switchToKey(int key)
    {
        switch(key)
        {
            case 1:
            wantedKey(sColor);
            Debug.Log("highlighted w");
            break;

            case 2:
            wantedKey(wColor);
            Debug.Log("highlighted d");
            break;

            case 3:
            wantedKey(dColor);
            Debug.Log("highlighted s");
            break;

            case 4:
            wantedKey(aColor);
            Debug.Log("highlighted a");
            break;

            default:
            fadeKeys();
            break;

        }

    }

    private void wantedKey(SpriteRenderer color)
    {
        fadeKeys();
        highlightKey(color, 1f);
    }

    private void highlightKey(SpriteRenderer color, float alpha)
    {
        Color tmp = color.color;
        
        tmp.a = alpha;

        color.color = tmp;

        Debug.Log("highlighted");
    }

    private void fadeKeys()
    {
        highlightKey(wColor, alphaValue);
        highlightKey(dColor, alphaValue);
        highlightKey(sColor, alphaValue);
        highlightKey(aColor, alphaValue);
        Debug.Log("Faded rest");
    }

}
