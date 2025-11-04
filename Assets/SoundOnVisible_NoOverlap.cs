using UnityEngine;
using System.Collections;
using Game.Audio;

public class SoundOnVisible_NoOverlap : MonoBehaviour
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
    private Coroutine _soundRoutine;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (_renderer.isVisible && !_isPlaying)
        {
            _soundRoutine = StartCoroutine(PlaySoundLoop());
        }
        else if (!_renderer.isVisible && _soundRoutine != null)
        {
            StopCoroutine(_soundRoutine);
            _soundRoutine = null;
            _isPlaying = false;
        }
    }

    private IEnumerator PlaySoundLoop()
    {
        _isPlaying = true;

        while (_renderer.isVisible)
        {
            // play sound and get AudioSource reference
            AudioSource src = SoundManagerNew.Instance.PlaySound(soundName, false);

            if (src != null && src.clip != null)
            {
                // wait until clip finishes
                yield return new WaitForSeconds(src.clip.length);
            }

            // wait for extra optional interval
            float waitTime = useRandomInterval
                ? Random.Range(randomMin, randomMax)
                : fixedInterval;

            yield return new WaitForSeconds(waitTime);
        }

        _isPlaying = false;
    }
}