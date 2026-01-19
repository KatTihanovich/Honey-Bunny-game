using UnityEngine;

public class NeckkerPatrolState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.IsChasing, false);
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("Run", false);
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        float waitTimer = blackboard.GetOrDefault<float>(BlackboardKeys.WaitTimer);
        
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            blackboard.Set(BlackboardKeys.WaitTimer, waitTimer);
            
            var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
            anim?.SetBool("Run", false);
            return;
        }
        
        bool movingLeft = blackboard.GetOrDefault<bool>(BlackboardKeys.MovingLeft);
        Transform leftEdge = blackboard.GetOrDefault<Transform>(BlackboardKeys.LeftEdge);
        Transform rightEdge = blackboard.GetOrDefault<Transform>(BlackboardKeys.RightEdge);
        Transform currentTarget = movingLeft ? leftEdge : rightEdge;
        float speed = blackboard.GetOrDefault<float>(BlackboardKeys.MoveSpeed);
        
        // Проверяем, достиг ли моб текущей точки патруля
        float distanceToTarget = Mathf.Abs(actor.transform.position.x - currentTarget.position.x);
        
        if (distanceToTarget < 0.2f)
        {
            // Достигли точки - останавливаемся и меняем направление
            StartWaiting(actor, blackboard);
        }
        else
        {
            // Движемся к текущей точке патруля
            int direction = movingLeft ? -1 : 1;
            MoveInDirection(actor, blackboard, direction, speed);
        }
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
    }

    private void MoveInDirection(GameObject actor, Blackboard blackboard, int direction, float speed)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        Vector3 baseScale = blackboard.GetOrDefault<Vector3>(BlackboardKeys.BaseScale);
        
        // Поворот
        Vector3 localScale = baseScale;
        localScale.x = direction > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        actor.transform.localScale = localScale;
        
        // Движение через transform (как в Burder)
        actor.transform.position += new Vector3(direction * speed * Time.deltaTime, 0f, 0f);
        
        anim?.SetBool("Run", true);
    }

    private void StartWaiting(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        float waitTime = blackboard.GetOrDefault<float>(BlackboardKeys.WaitTimeAtPoint);
        bool movingLeft = blackboard.GetOrDefault<bool>(BlackboardKeys.MovingLeft);
        
        anim?.SetBool("Run", false);
        
        blackboard.Set(BlackboardKeys.MovingLeft, !movingLeft);
        blackboard.Set(BlackboardKeys.WaitTimer, waitTime);
    }
}
