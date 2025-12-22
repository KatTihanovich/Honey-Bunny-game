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

        if (string.IsNullOrEmpty(ageInput.text))
        {
            ShowMessage("Введите возраст!", Color.red);
            return;
        }

        int age;
        if (!int.TryParse(ageInput.text, out age) || age < 1 || age > 120)
        {
            ShowMessage("Некорректный возраст!", Color.red);
            return;
        }

        // Отключаем кнопку во время запроса
        registerButton.interactable = false;
        ShowMessage("Регистрация...", Color.yellow);

        // Вызываем API
        StartCoroutine(GameAPIManager.Instance.Register(
            nicknameInput.text,
            passwordInput.text,
            age,
            OnRegisterResponse
        ));
    }

    void OnRegisterResponse(bool success, string response)
    {
        registerButton.interactable = true;

        if (success)
        {
            ShowMessage("Регистрация успешна!", Color.green);
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
