using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitButtonScript : MonoBehaviour
{
   public Button settings_button;        
    public Button info_button;       
    public Button arrow_button;     
    
    public void Exit()
    {
        settings_button.interactable = true;
        info_button.interactable = true;
        arrow_button.interactable = true;

        SceneManager.LoadSceneAsync(0);
    }

}
