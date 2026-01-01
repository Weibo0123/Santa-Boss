using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    [Header("Chase")]
    public Transform player;
    [SerializeField] float chaseRange = 8f;
    [Header("Leap")]
    [SerializeField] float leapTriggerHeight = 1.5f;
    [SerializeField] float leapTriggerDistance = 8f;
    [Header("Punch")] 
    [SerializeField] float punchCooldown = 10f;
    [Header("Exception Handling")]
    [SerializeField] float stuckTimeLimit = 5f;
    [SerializeField] float minMoveThreshold = 0.5f;
    float stuckTimer = 0f;
    Vector2 lastPosition;

    float punchTimer = 10f;
    enum BossState
    {
        Idle,
        Chasing,
        Leaping,
        TeleportPunching
    }

    BossState currentState = BossState.Idle;
    BossMovement movement;
    BossLeap leap;
    BossPunch punch;
    Rigidbody2D rb;
    bool leapStarted = false;

    void Start()
    {
        movement = GetComponent<BossMovement>();
        leap = GetComponent<BossLeap>();
        punch = GetComponent<BossPunch>();
        rb = GetComponent<Rigidbody2D>();
        leap.onLeapFinished = onLeapFinished;
        lastPosition = rb.position;
    }

    float distanceToPlayer;
    void FixedUpdate()
    {
        float dy = player.position.y - transform.position.y;
        distanceToPlayer = player.position.x - transform.position.x;
        punchTimer = Mathf.Max(0, punchTimer - Time.fixedDeltaTime);
        if (transform.position.y < -4f)
        {
            // Reset boss if it falls off the map
            HandleException();
        }
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
                TrySwitchToPunching();
                // TrySwitchToIdle();
                break;

            case BossState.Leaping:
                if (!leapStarted)
                    {
                        leapStarted = true;
                        leap.leap(player);
                    }
                break;

            case BossState.TeleportPunching:
                punch.TryPunch(player, onPunchFinished);
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
        if ( DistanceToPlayer() > chaseRange)
            currentState = BossState.Idle;
    }

    void TrySwitchToPunching()
    {
        bool isGrounded = movement.IsGrounded();
        if (punchTimer <= 0 && punch.canPunch && isGrounded)
        {
            currentState = BossState.TeleportPunching;
        }
        
    }

    void onLeapFinished()
    {
        currentState = BossState.Chasing;
        leapStarted = false;
    }

    void onPunchFinished()
    {
        currentState = BossState.Chasing;
        punchTimer = punchCooldown;
    }

    float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    void HandleStuck()
    {
        float moved = Vector2.Distance(rb.position, rb.position);
        if (moved < minMoveThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = rb.position;

        if (stuckTimer >= stuckTimeLimit)
        {
            HandleException();
            stuckTimer = 0f;
        }
    }

    void HandleException()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        currentState = BossState.TeleportPunching;
    }
}
