using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossFightGlobal : MonoBehaviour
{
    private static readonly int RescuedTrigger = Animator.StringToHash("Rescued");

    private GameObject receiver;
    private GameObject cage;
    private Animator animator;
 
    public Health bossHealth;

    [Header("Cutscene")]
    public VideoPlayer cutsceneVideo;
    public GameObject UICanvas;
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    private const string musicVolumeParam = "MusicVolume";

    void Start()
    {
        receiver = GameObject.Find("Bunny");
        if (receiver == null) Debug.LogError("[Cage] Bunny not found!!!");

        cage = GameObject.FindWithTag("Honey");
        if (cage == null) Debug.LogError("[Cage] Honey not found!!!");

        animator = cage.GetComponent<Animator>();

        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("СВОБОДА");
            animator.SetTrigger(RescuedTrigger);

            // Save boss defeated state
            PlayerPrefs.SetInt("BossDefeated", 1);
            PlayerPrefs.Save();

            if (cutsceneVideo != null && fadePanel != null)
            {
                StartCoroutine(PlayCutscene());
            }
            else
            {
                Debug.LogError("VideoPlayer или fadePanel не назначены!");
            }
        }
    }

    private IEnumerator PlayCutscene()
    {
        // Immediately mute background music
        if (audioMixer != null) audioMixer.SetFloat(musicVolumeParam, -80f);

        // Smooth fade to black
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {
            panelColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadePanel.color = panelColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelColor.a = 1f;
        fadePanel.color = panelColor;

        // Enable and play video
        if (cutsceneVideo != null)
        {
            cutsceneVideo.gameObject.SetActive(true);
            cutsceneVideo.Prepare();
            yield return new WaitUntil(() => cutsceneVideo.isPrepared);

            UICanvas.SetActive(false);
            cutsceneVideo.Play();

            // Add listener to load the main menu after the video ends
            cutsceneVideo.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned!");
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Restore audio when returning to MainMenu
        if (audioMixer != null) audioMixer.SetFloat(musicVolumeParam, 0f);

        SceneManager.LoadScene("MainMenu");
    }
}