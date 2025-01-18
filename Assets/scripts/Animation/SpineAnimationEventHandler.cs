using UnityEngine;
using Spine;
using Spine.Unity;

public class SpineAnimationEventHandler : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    void Start()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
    }

    void OnDestroy()
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Event -= HandleAnimationEvent;
        }
    }
}
