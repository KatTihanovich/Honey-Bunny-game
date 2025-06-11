using UnityEngine;
using Game.Audio;

public class StonePushSound : StateMachineBehaviour
{
    private AudioSource _loopingSource;
  
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       _loopingSource = SoundManagerNew.Instance.PlaySound("StonePush", loop: true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_loopingSource != null)
        {
            SoundManagerNew.Instance.StopSound(_loopingSource);
        }
    }
}
