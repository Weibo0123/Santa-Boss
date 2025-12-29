using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    public Transform player;
    [SerializeField] float chaseRange = 8f;
    [SerializeField] float leapTriggerHeight = 1.5f;
    [SerializeField] float leapTriggerDistance = 8f;

    enum BossState
    {
        Idle,
        Chasing,
        Leaping
    }

    BossState currentState = BossState.Idle;
    BossMovement movement;
    BossLeap leap;
    bool leapStarted = false;

    void Start()
    {
        movement = GetComponent<BossMovement>();
        leap = GetComponent<BossLeap>();
        leap.onLeapFinished = onLeapFinished;
    }

    float distanceToPlayer;
    void FixedUpdate()
    {
        float dy = player.position.y - transform.position.y;
        distanceToPlayer = player.position.x - transform.position.x;
        switch (currentState)
        {
            case BossState.Idle:
                movement.Stop();
                TrySwitchToChasing();
                break;

            case BossState.Chasing:
                movement.Chase(player);
                movement.TryJump(player);
                TrySwitchToLeap(dy);
                TrySwitchToIdle();
                break;

            case BossState.Leaping:
                if (!leapStarted)
                    {
                        leapStarted = true;
                        leap.leap(player);
                    }
                break;
        }
    }

void TrySwitchToLeap(float dy)
    {
        if (leap.CanLeap() && (dy > leapTriggerHeight || Mathf.Abs(distanceToPlayer) > leapTriggerDistance))
        {
            currentState = BossState.Leaping;
        }
    }

    void TrySwitchToChasing()
    {
        if (DistanceToPlayer() <= chaseRange)
            currentState = BossState.Chasing;
    }

    void TrySwitchToIdle()
    {
        if (DistanceToPlayer() > chaseRange)
            currentState = BossState.Idle;
    }

    void onLeapFinished()
    {
        currentState = BossState.Chasing;
        leapStarted = false;
    }

    float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }
}
