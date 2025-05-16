using UnityEngine;
using System.Collections;
using Game.Audio;

public class SoundOnVisible : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private string soundName;

    [Header("Timing Settings")]
    public bool useRandomInterval = false;
    public float fixedInterval = 1.5f;
    public float randomMin = 0.5f;
    public float randomMax = 2.5f;

    private Renderer _renderer;
    private bool _isPlaying;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (_renderer.isVisible && !_isPlaying)
        {
            StartCoroutine(PlaySoundLoop());
        }
    }

    private IEnumerator PlaySoundLoop()
    {
        _isPlaying = true;

        while (_renderer.isVisible)
        {
            SoundManagerNew.Instance.PlaySound(soundName);

            float waitTime = useRandomInterval
                ? Random.Range(randomMin, randomMax)
                : fixedInterval;

            yield return new WaitForSeconds(waitTime);
        }

        _isPlaying = false;
    }
}