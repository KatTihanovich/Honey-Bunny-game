using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserProfileButton : MonoBehaviour
{
    public Button profileButton;
    public GameObject userInfoCanvas; // Canvas который открывается
    public TMP_Text messageText; // Текст для сообщения об ошибке (опционально)

    void Start()
    {
        profileButton.onClick.AddListener(OnProfileButtonClicked);
    }

    void OnProfileButtonClicked()
    {
        // Проверяем авторизацию
        bool isLoggedIn = PlayerPrefs.HasKey("JWT_Token") && 
                         !string.IsNullOrEmpty(PlayerPrefs.GetString("JWT_Token"));

        if (isLoggedIn)
        {
            // Пользователь авторизован - открываем профиль
            if (userInfoCanvas != null)
            {
                userInfoCanvas.SetActive(true);
            }
        }
        else
        {
            // Пользователь НЕ авторизован - показываем сообщение
            ShowLoginMessage();
        }
    }

    void ShowLoginMessage()
    {
        // Если есть отдельный Text для сообщений
        if (messageText != null)
        {
            messageText.text = "Пожалуйста, войдите в аккаунт!";
            messageText.color = Color.red;
            
            // Сообщение исчезнет через 3 секунды
            Invoke("ClearMessage", 3f);
        }
        else
        {
            // Просто выводим в консоль
            Debug.Log("Необходима авторизация!");
        }
    }

    void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}
