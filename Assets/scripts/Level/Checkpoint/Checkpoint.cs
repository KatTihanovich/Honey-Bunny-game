using UnityEngine;
using Spine.Unity; 

public class Checkpoint : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;

    private void Start()
    {
     
        skeletonAnim = GetComponentInChildren<SkeletonAnimation>();
        Debug.Log(skeletonAnim);
        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, "light off", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (collision.gameObject.CompareTag("Player"))
        {
       
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, "light on", false);
        }

  
    }
}
