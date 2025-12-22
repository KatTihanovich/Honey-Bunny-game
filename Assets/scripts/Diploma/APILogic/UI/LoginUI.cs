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
        // Валидация
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            ShowMessage("Введите никнейм!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            ShowMessage("Введите пароль!", Color.red);
            return;
        }

        // Отключаем кнопку во время запроса
        loginButton.interactable = false;
        ShowMessage("Вход...", Color.yellow);

        // Вызываем API
        StartCoroutine(GameAPIManager.Instance.Login(
            nicknameInput.text,
            passwordInput.text,
            OnLoginResponse
        ));
    }

    void OnLoginResponse(bool success, string response)
    {
        loginButton.interactable = true;

        if (success)
        {
            ShowMessage("Вход выполнен!", Color.green);
        }
        else
        {
            ShowMessage("Ошибка: " + response, Color.red);
        }
    }

    void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}
