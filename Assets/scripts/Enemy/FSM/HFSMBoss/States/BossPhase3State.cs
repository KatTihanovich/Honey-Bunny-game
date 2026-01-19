using UnityEngine;
using Enemy;

public class BossPhase3State : CompositeState
{
    private float tailSpawnInterval = 10f;
    private float tailSpawnTimer = 0f;
    private float tailOffsetX = 15f;

    private BossChaseState chase;
    private BossRoarState  roar;

    public BossPhase3State()
    {
        chase = new BossChaseState();
        roar  = new BossRoarState();

        AddSubState(chase);
        AddSubState(roar);
        InitialState = roar;
    }

    public override void Enter(GameObject actor, Blackboard blackboard)
    {
        base.Enter(actor, blackboard);
        Debug.Log("[Phase3] MADNESS!");
        tailSpawnTimer = 0f;

        blackboard.Set(BlackboardKeys.HasRoared, false);
    }

    public override void Tick(GameObject actor, Blackboard blackboard)
        {
            base.Tick(actor, blackboard);
            tailSpawnTimer += Time.deltaTime;

            if (blackboard.GetOrDefault<bool>(BlackboardKeys.HasRoared) &&
                SubStateMachine.CurrentState == roar)
            {
                SubStateMachine.ChangeState(chase);
            }

            if (tailSpawnTimer >= tailSpawnInterval)
            {
                SpawnTails(actor, blackboard);
                tailSpawnTimer = 0f;
            }
        }

    private void SpawnTails(GameObject actor, Blackboard blackboard)
    {
        var tails  = blackboard.GetOrDefault<TailBossEnemyScript[]>(BlackboardKeys.BossTails);
        var player = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);

        if (tails == null || tails.Length < 2 || player == null) return;

        Vector3 leftPos  = player.position + new Vector3(-tailOffsetX, 0, 0);
        Vector3 rightPos = player.position + new Vector3( tailOffsetX, 0, 0);

        if (tails[0].gameObject.activeInHierarchy)
        {
            tails[0].transform.position = leftPos;
            tails[0].RespawnOrAppear();
        }

        if (tails[1].gameObject.activeInHierarchy)
        {
            tails[1].transform.position = rightPos;
            tails[1].RespawnOrAppear();
        }
    }
}
