using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI Elements")]
    public GameObject tutorialCanvas; 
    public GameObject[] tutorialPanels; 
    private int currentPanelIndex = 0; 

    private void Start()
    {
        ShowPanel(0);
    }

    private void ShowPanel(int index)
    {
        foreach (var panel in tutorialPanels)
        {
            panel.SetActive(false);
        }

        if (index >= 0 && index < tutorialPanels.Length)
        {
            tutorialPanels[index].SetActive(true);
            currentPanelIndex = index;
        }
        else
        {
            tutorialCanvas.SetActive(false);
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
