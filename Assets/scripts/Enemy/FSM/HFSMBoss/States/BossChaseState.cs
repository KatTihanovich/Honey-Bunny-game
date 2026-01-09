using System.Collections;
using UnityEngine;

public class BossChaseState : CompositeState
{
    private enum Phase
    {
        Appear,
        Surface,
        Disappear,
        Wait
    }

    private Phase currentPhase;
    private float timer;

    private const float appearDuration    = 0.9f;
    private const float surfaceDuration   = 5f;
    private const float disappearDuration = 0.9f;
    private const float waitDuration      = 8f;

    private float groundY;
    private BossSurfaceState surfaceState;

    private MeshRenderer meshRenderer;


    public BossChaseState()
    {
        surfaceState = new BossSurfaceState();
        AddSubState(surfaceState);
    }

    public override void Enter(GameObject actor, Blackboard blackboard)
    {
        currentPhase = Phase.Appear;
        timer = 0f;

        groundY = actor.transform.position.y;

        TeleportUnderPlayerX(actor, blackboard);

        blackboard.Set(BlackboardKeys.IsVisible, false);
        blackboard.Set(BlackboardKeys.CanTakeDamage, false);
        meshRenderer = actor.GetComponent<MeshRenderer>();


        blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator)
            ?.SetTrigger("Appear");

        Debug.Log("[BossChase] Appear");
    }

    public override void Tick(GameObject actor, Blackboard blackboard)
    {
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.Appear:
                if (timer >= appearDuration)
                    EnterSurface(actor, blackboard);
                break;

            case Phase.Surface:
                surfaceState.Tick(actor, blackboard);

                if (timer >= surfaceDuration)
                    EnterDisappear(actor, blackboard);
                break;

            case Phase.Disappear:
                if (timer >= disappearDuration)
                    EnterWait();
                break;

            case Phase.Wait:
                if (timer >= waitDuration)
                    EnterAppear(actor, blackboard);
                break;
        }
    }

    // =========================
    // PHASE TRANSITIONS
    // =========================

    private void EnterSurface(GameObject actor, Blackboard blackboard)
    {
        currentPhase = Phase.Surface;
        timer = 0f;

        surfaceState.Enter(actor, blackboard);

        Debug.Log("[BossChase] Surface");
    }

    private void EnterDisappear(GameObject actor, Blackboard blackboard)
    {
        currentPhase = Phase.Disappear;
        timer = 0f;

        surfaceState.Exit(actor, blackboard);

        blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator)
            ?.SetTrigger("Hide");

        Debug.Log("[BossChase] Disappear");
    }

    private void EnterWait()
    {
        meshRenderer.enabled = false;
        currentPhase = Phase.Wait;
        timer = 0f;
        Debug.Log("[BossChase] Wait");
    }

    private void EnterAppear(GameObject actor, Blackboard blackboard)
    {
        currentPhase = Phase.Appear;
        timer = 0f;
        meshRenderer.enabled = true;
        TeleportUnderPlayerX(actor, blackboard);

        blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator)
            ?.SetTrigger("Appear");

        Debug.Log("[BossChase] Appear");
    }

    // =========================
    // HELPERS
    // =========================
    private void TeleportUnderPlayerX(GameObject actor, Blackboard blackboard)
    {
        bool isFirstAppear = blackboard.GetOrDefault<bool>(BlackboardKeys.IsFirstAppear, true);
        if (isFirstAppear)
        {
            var spawnPoint = blackboard.GetOrDefault<Transform>(BlackboardKeys.BossSpawnPoint);
            if (spawnPoint != null)
            {
                Vector3 pos = actor.transform.position;
                pos.x = spawnPoint.position.x;
                pos.y = groundY;   
                actor.transform.position = pos;

                blackboard.Set(BlackboardKeys.IsFirstAppear, false);
                return;
            }
        }

        var player = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (player == null) return;

        Vector3 posPlayer = actor.transform.position;
        posPlayer.x = player.position.x + Random.Range(-2f, 2f);
        posPlayer.y = groundY;

        actor.transform.position = posPlayer;
    }

}
