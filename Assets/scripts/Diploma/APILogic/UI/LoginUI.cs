using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nicknameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text messageText;
public UserProfileButton profileButton;

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        messageText.text = "";
    }

    void OnLoginClicked()
    {
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            ShowMessage("Insert nickname!", Color.gray);
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            ShowMessage("Insert password!", Color.gray);
            return;
        }

        loginButton.interactable = false;
        ShowMessage("Logging in...", Color.gray);

        StartCoroutine(GameAPIManager.Instance.Login(
            nicknameInput.text,
            passwordInput.text,
            OnLoginResponse
        ));
    }

    void OnLoginResponse(bool success, string response, long code)
    {
        loginButton.interactable = true;

            if (success)
            {
                ShowMessage("Login successful!", Color.gray);
                return;
            }

            switch (code)
            {
                case 400:
                case 401:
                    ShowMessage("Invalid credentials", Color.gray);
                    break;

                case 0:
                    ShowMessage("No internet connection", Color.gray);
                    break;

                default:
                    ShowMessage("Something went wrong. Please try again.", Color.gray);
                    break;
            }
    }



    void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}
