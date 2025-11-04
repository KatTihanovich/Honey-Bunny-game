using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

public class UIButtonSound : MonoBehaviour
{
    private ISoundManager _soundManager;
    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;
    }

    public void PlayClickSound()
    {
        _soundManager.PlaySound("UiClick");
    }


}
