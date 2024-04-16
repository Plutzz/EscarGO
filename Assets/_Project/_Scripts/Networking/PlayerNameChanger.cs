using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerNameChanger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TMP_InputField inputField;
    public void ChangePlayerName()
    {
        if(AuthenticationService.Instance.IsSignedIn && inputField.text != "")
        {
            ChangePlayerNameAsync(inputField.text);
        }
    }

    private async void ChangePlayerNameAsync(string playerName)
    {
        string newUsername = await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        usernameText.text = newUsername.Substring(0, newUsername.Length - 5);
    }
}
