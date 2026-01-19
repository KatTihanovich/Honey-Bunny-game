using UnityEngine;

public class BurderRunState : IState
{

    public IState ParentState { get; set; }
    public bool IsComposite => false;
    private bool _hasTriggeredSwitch = false;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        _hasTriggeredSwitch = false;
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetTrigger("Switch_to_run");
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        if (!_hasTriggeredSwitch)
        {
            _hasTriggeredSwitch = true;
            var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
            anim?.SetTrigger("Run");
        }

        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return;

        RotateTowardsPlayer(actor, blackboard);
        MoveTowards(actor, blackboard, playerTransform.position);

        float attackTimer = blackboard.GetOrDefault<float>(BlackboardKeys.AttackTimer);
        attackTimer += Time.deltaTime;
        blackboard.Set(BlackboardKeys.AttackTimer, attackTimer);
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
    }

    private void MoveTowards(GameObject actor, Blackboard blackboard, Vector3 target)
    {
        float fleeSpeed = blackboard.GetOrDefault<float>(BlackboardKeys.FleeSpeed);
        Vector3 direction = target - actor.transform.position;
        direction.y = 0f;
        direction = direction.normalized;
        
        actor.transform.position += new Vector3(direction.x, 0f, 0f) * fleeSpeed * Time.deltaTime;
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
