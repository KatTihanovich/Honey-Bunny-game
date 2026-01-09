using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nicknameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button backButton;
    public TMP_Text messageText;

    void Start()
    {
        loginButton.onClick.AddListener(OnUpdateClicked);
        backButton.onClick.AddListener(OnBackClicked);
        messageText.text = "";
    }

    void OnUpdateClicked()
    {
        // Validate nickname
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            ShowMessage("Insert new nickname!", Color.red);
            return;
        }

        // Validate password
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            ShowMessage("Insert new password!", Color.red);
            return;
        }

        // Password strength validation
        if (passwordInput.text.Length < 6)
        {
            ShowMessage("Password must be at least 6 characters!", Color.red);
            return;
        }

        loginButton.interactable = false;
        ShowMessage("Updating profile...", Color.gray);

        StartCoroutine(GameAPIManager.Instance.UpdateUser(
            nicknameInput.text,
            passwordInput.text,
            OnUpdateResponse
        ));
    }

    void OnUpdateResponse(bool success, string response)
    {
        loginButton.interactable = true;

        if (success)
        {
            ShowMessage("Profile updated successfully!", Color.green);
            
            // Save new nickname
            PlayerPrefs.SetString("User_Nickname", nicknameInput.text);
            PlayerPrefs.Save();
            
            // Clear input fields
            ClearInputFields();
        }
        else
        {
            ShowMessage("Error: " + response, Color.red);
        }
    }

    void OnBackClicked()
    {
        ClearInputFields();
        ShowMessage("", Color.white);
        // Add scene navigation if needed
        // SceneManager.LoadScene("PreviousScene");
    }

    void ClearInputFields()
    {
        nicknameInput.text = "";
        passwordInput.text = "";
    }

    void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}
