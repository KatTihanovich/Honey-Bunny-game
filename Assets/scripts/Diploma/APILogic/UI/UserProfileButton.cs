using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserProfileButton : MonoBehaviour
{
    public Button profileButton;
    public GameObject userInfoCanvas; 
    public TMP_Text messageText; 

    void Start()
    {
        profileButton.onClick.AddListener(OnProfileButtonClicked);
    }

    void OnProfileButtonClicked()
    {
        bool isLoggedIn = PlayerPrefs.HasKey("JWT_Token") && 
                         !string.IsNullOrEmpty(PlayerPrefs.GetString("JWT_Token"));

        if (isLoggedIn)
        {
            if (userInfoCanvas != null)
            {
                userInfoCanvas.SetActive(true);
            }
        }
        else
        {
            ShowLoginMessage();
        }
    }

    void ShowLoginMessage()
    {
        if (messageText != null)
        {
            messageText.text = "Please, login!";
            messageText.color = Color.red;
            
            Invoke("ClearMessage", 3f);
        }
        else
        {
            Debug.Log("Authorization is required!");
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
