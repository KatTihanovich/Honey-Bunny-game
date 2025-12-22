using UnityEngine;

public class BurderWalkState : IState
{
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.IsChasing, true);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetTrigger("Walk");
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return;

        RotateTowardsPlayer(actor, blackboard);
        MoveTowards(actor, blackboard, playerTransform.position);
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
    }

    private void MoveTowards(GameObject actor, Blackboard blackboard, Vector3 target)
    {
        float moveSpeed = blackboard.GetOrDefault<float>(BlackboardKeys.MoveSpeed);
        Vector3 direction = target - actor.transform.position;
        direction.y = 0f;
        direction = direction.normalized;
        
        actor.transform.position += new Vector3(direction.x, 0f, 0f) * moveSpeed * Time.deltaTime;
    }

    private void RotateTowardsPlayer(GameObject actor, Blackboard blackboard)
    {
        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return;

        Vector3 baseScale = blackboard.GetOrDefault<Vector3>(BlackboardKeys.BaseScale);
        Vector3 direction = playerTransform.position - actor.transform.position;
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        actor.transform.localScale = localScale;
    }
}
