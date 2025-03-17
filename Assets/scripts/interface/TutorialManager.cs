using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI Elements")]
    public GameObject tutorialCanvas; 
    public GameObject[] tutorialPanels; 
    public Button[] selectableButtons; // Buttons to select for each panel
    private int currentPanelIndex = 0; 

    private void Start()
    {
        ShowPanel(0);
    }

    private void ShowPanel(int index)
    {
        // Deactivate all panels
        foreach (var panel in tutorialPanels)
        {
            panel.SetActive(false);
        }

        // Activate target panel and select its button
        if (index >= 0 && index < tutorialPanels.Length)
        {
            tutorialPanels[index].SetActive(true);
            currentPanelIndex = index;
            SelectButton(index);
        }
        else
        {
            tutorialCanvas.SetActive(false);
        }
    }

    private void SelectButton(int index)
    {
        if (index >= 0 && index < selectableButtons.Length && selectableButtons[index] != null)
        {
            EventSystem.current.SetSelectedGameObject(selectableButtons[index].gameObject);
        }
    }

    public void SkipTutorial()
    {
        tutorialCanvas.SetActive(false);
    }

    public void NextPanel()
    {
        ShowPanel(currentPanelIndex + 1);
    }
}
