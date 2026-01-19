using UnityEngine;

public class BurderPatrolState : IState
{

    public IState ParentState { get; set; }
    public bool IsComposite => false;
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.IsChasing, false);
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        blackboard.Set(BlackboardKeys.ThornSpawnFinished, true);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetTrigger("Walk");
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        bool isSpawningThorn = !blackboard.GetOrDefault(BlackboardKeys.ThornSpawnFinished, true);
        if (isSpawningThorn) return;
        
        // Check thorn cooldown
        float thornTimer = blackboard.GetOrDefault<float>(BlackboardKeys.ThornTimer);
        float thornCooldown = blackboard.GetOrDefault<float>(BlackboardKeys.ThornCooldown);
        bool canSpawnThorns = blackboard.GetOrDefault<bool>(BlackboardKeys.CanSpawnThorns);
        
        thornTimer += Time.deltaTime;
        blackboard.Set(BlackboardKeys.ThornTimer, thornTimer);
        
        if (canSpawnThorns && thornTimer >= thornCooldown)
        {
            SpawnThorn(actor, blackboard);
            blackboard.Set(BlackboardKeys.ThornTimer, 0f);
            return;
        }
        
        // Patrol logic
        Patrol(actor, blackboard);
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
    }

    private void Patrol(GameObject actor, Blackboard blackboard)
    {
        Transform currentPatrolPoint = blackboard.GetOrDefault<Transform>(BlackboardKeys.CurrentPatrolPoint);
        Transform pointA = blackboard.GetOrDefault<Transform>(BlackboardKeys.PointA);
        Transform pointB = blackboard.GetOrDefault<Transform>(BlackboardKeys.PointB);
        float moveSpeed = blackboard.GetOrDefault<float>(BlackboardKeys.MoveSpeed);
        Vector3 baseScale = blackboard.GetOrDefault<Vector3>(BlackboardKeys.BaseScale);

        if (currentPatrolPoint == null) return;

        Vector3 direction = currentPatrolPoint.position - actor.transform.position;
        direction.y = 0f;
        direction = direction.normalized;
        
        actor.transform.position += new Vector3(direction.x, 0f, 0f) * moveSpeed * Time.deltaTime;

        float distance = Mathf.Abs(actor.transform.position.x - currentPatrolPoint.position.x);
        if (distance < 0.2f)
        {
            currentPatrolPoint = currentPatrolPoint == pointA ? pointB : pointA;
            blackboard.Set(BlackboardKeys.CurrentPatrolPoint, currentPatrolPoint);
        }

        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        actor.transform.localScale = localScale;
    }

    private void SpawnThorn(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.ThornSpawnFinished, false);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetTrigger("SpawnThorn");
    }
}
