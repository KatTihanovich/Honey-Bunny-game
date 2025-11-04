using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Audio;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    //public GameObject toSelect;
    public int passedLevelNumber;
    private ISoundManager _soundManager;

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //EventSystem.current.SetSelectedGameObject(toSelect);
            _soundManager.PlaySound("Finish");
            endScreen.SetActive(true);
            Time.timeScale = 0f;
            MarkLevelComplete(passedLevelNumber);
            Debug.Log("LevelUnlocked = " + PlayerPrefs.GetInt("LevelUnlocked"));
        }
    }

    public void MarkLevelComplete(int levelNumber)
    {
        int current = PlayerPrefs.GetInt("LevelUnlocked", 1);
        if (levelNumber + 1 > current)
        {
            PlayerPrefs.SetInt("LevelUnlocked", levelNumber + 1);
            PlayerPrefs.Save();
        }
    }

}
