using UnityEngine;
using Game.Audio;
using System.Collections.Generic;

public class StonePushSound : StateMachineBehaviour
{
    private AudioSource _loopingSource;

    public static readonly List<AudioSource> ActiveStonePushSources = new List<AudioSource>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _loopingSource = SoundManagerNew.Instance.PlaySound("StonePush", loop: true);
        if (_loopingSource != null)
            ActiveStonePushSources.Add(_loopingSource);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_loopingSource != null)
        {
            SoundManagerNew.Instance.StopSound(_loopingSource);
            ActiveStonePushSources.Remove(_loopingSource);
        }
    }
}
