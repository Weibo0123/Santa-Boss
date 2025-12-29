using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    public Transform player;
    [SerializeField] float chaseRange = 8f;

    enum BossState
    {
        Idle,
        Chasing,
        Leaping
    }

    BossState currentState = BossState.Idle;
    BossMovement movement;

    void Start()
    {
        movement = GetComponent<BossMovement>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case BossState.Idle:
                movement.Stop();
                TrySwitchToChasing();
                break;

            case BossState.Chasing:
                movement.Chase(player);
                movement.TryJump(player);
                TrySwitchToIdle();
                break;

            case BossState.Leaping:
                // Placeholder for future leap behavior
                break;
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

    float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }
}
