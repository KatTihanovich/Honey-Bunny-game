using UnityEngine;
using TMPro;

public class PlayCounterDisplay : MonoBehaviour
{
    public TextMeshProUGUI startGameText;
    public TextMeshProUGUI restartText;

    void Update()
    {
        startGameText.text = "Начато игр: " + PlayCounter.GetStartGameCount();
        restartText.text   = "Рестартов: " + PlayCounter.GetRestartCount();
    }
}
