using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegistrationUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField  nicknameInput;
    public TMP_InputField  passwordInput;
    public TMP_InputField ageInput;
    public Button registerButton;
    public TMP_Text messageText;
public UserProfileButton profileButton;
    void Start()
    {
        registerButton.onClick.AddListener(OnRegisterClicked);
        messageText.text = "";
    }

    void OnRegisterClicked()
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

        int age;
        if (!int.TryParse(ageInput.text, out age) || age < 10 || age > 120)
        {
            ShowMessage("Invalid age!", Color.gray);
            return;
        }

        registerButton.interactable = false;
        ShowMessage("Registering...", Color.gray);

        StartCoroutine(GameAPIManager.Instance.Register(
            nicknameInput.text,
            passwordInput.text,
            age,
            OnRegisterResponse
        ));
    }

    void OnRegisterResponse(bool success, string response, long code)
    {
        registerButton.interactable = true;

        if (success)
        {
                ShowMessage("Registration successful!", Color.gray);
        }
        else
        {
            ShowMessage("Error: " + response, Color.gray);
        }
    }

    void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}
