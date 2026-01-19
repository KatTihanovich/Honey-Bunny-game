using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeleteAccountUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button yesButton;
    public Button noButton;
    public TMP_Text messageText;
    
    [Header("Canvas Navigation")]
    public GameObject deleteAccountCanvas;
    public GameObject loginCanvas;

    void Start()
    {
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
        
        if (messageText != null)
        {
            messageText.text = "Are you sure you want to delete your account? This action cannot be undone!";
            messageText.color = Color.gray;
        }
    }

    void OnYesClicked()
    {
        yesButton.interactable = false;
        noButton.interactable = false;
        
        if (messageText != null)
        {
            ShowMessage("Deleting account...", Color.gray);
        }

        StartCoroutine(GameAPIManager.Instance.DeleteUser(OnDeleteResponse));
    }

    void OnDeleteResponse(bool success, string response)
    {
        if (success)
        {
            if (messageText != null)
            {
                ShowMessage("Account deleted successfully!", Color.gray);
            }
            
            Debug.Log("Account deleted, redirecting to login");
            
            Invoke("RedirectToLogin", 1.5f);
        }
        else
        {
            yesButton.interactable = true;
            noButton.interactable = true;
            
            if (messageText != null)
            {
                ShowMessage("Error: " + response, Color.gray);
            }
        }
    }

    void OnNoClicked()
    {
        Debug.Log("Account deletion cancelled");
        
        if (deleteAccountCanvas != null)
        {
            deleteAccountCanvas.SetActive(false);
        }
    }

    void RedirectToLogin()
    {
        if (deleteAccountCanvas != null)
        {
            deleteAccountCanvas.SetActive(false);
        }
        
        if (loginCanvas != null)
        {
            loginCanvas.SetActive(true);
        }
    }

    void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}
