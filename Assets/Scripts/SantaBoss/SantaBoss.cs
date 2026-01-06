using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    // Boss parameters
    [Header("Chase")]
    public Transform player; // Reference to player transform
    [SerializeField] float chaseRange = 8f; // Distance to start chasing player
    [Header("Leap")]
    [SerializeField] float leapTriggerHeight = 1.5f; // Vertical distance to trigger leap
    [SerializeField] float leapTriggerDistance = 8f; // Horizontal distance to trigger leap
    [Header("Punch")] 
    [SerializeField] float punchCooldown = 10f; // Time between punches
    [Header("Exception Handling")]
    [SerializeField] float stuckTimeLimit = 5f; // Time boss can be stuck before exception handling
    [SerializeField] float minMoveThreshold = 0.5f; // Minimum distance to consider as movement
    float stuckTimer = 0f; // Timer to track how long the boss has been stuck
    Vector2 lastPosition; // Last recorded position of the boss

    float punchTimer = 10f; // Timer to track punch cooldown
    enum BossState // Current states of the boss
    {
        Idle, // Not moving
        Chasing,  // Moving towards player
        Leaping, // Performing a leap attack
        TeleportPunching // Performing a teleport punch attack
    }

    BossState currentState = BossState.Idle; // Initial state
    BossMovement movement; // Reference to movement script
    BossLeap leap; // Reference to leap script
    BossPunch punch; // Reference to punch script
    Rigidbody2D rb;
    bool leapStarted = false; // Flag to track if leap has started

    void Start()
    {
        // Get references to components
        movement = GetComponent<BossMovement>();
        leap = GetComponent<BossLeap>();
        punch = GetComponent<BossPunch>();
        rb = GetComponent<Rigidbody2D>();
        // Register leap finished callback
        leap.onLeapFinished = onLeapFinished;
        // Record initial position  
        lastPosition = rb.position;
    }

    float distanceToPlayer;
    void FixedUpdate()
    {
        // Calculate vertical distance to player
        float dy = player.position.y - transform.position.y;
        // Update distance to player
        distanceToPlayer = player.position.x - transform.position.x;
        // Update the punch timer cooldown
        punchTimer = Mathf.Max(0, punchTimer - Time.fixedDeltaTime);
        // Handle stuck detection
        if (transform.position.y < -4f)
        {
            // Use punch if boss falls off the map
            HandleException();
        }
        // Main state machine
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
                break;

            case BossState.Leaping:
            // Ensure leap is only started once
                if (!leapStarted)
                    {
                        leapStarted = true;
                        leap.leap(player);
                    }
                break;

            case BossState.TeleportPunching:
            // Attempt to punch the player
                punch.TryPunch(player, onPunchFinished);
                break;
        }
    }

    // States switching methods
    void TrySwitchToLeap(float dy)
    {
        // Trigger leap if player is high enough or far enough
        if (leap.CanLeap() && (dy > leapTriggerHeight || Mathf.Abs(distanceToPlayer) > leapTriggerDistance))
        {
            currentState = BossState.Leaping;
        }
    }

    void TrySwitchToChasing()
    {
        // Switch to chasing if player is within range
        if (DistanceToPlayer() <= chaseRange)
            currentState = BossState.Chasing;
    }

    void TrySwitchToPunching()
    {
        // Check if punch can be performed
        bool isGrounded = movement.IsGrounded();
        if (punchTimer <= 0 && punch.canPunch && isGrounded)
        {
            currentState = BossState.TeleportPunching;
        }
        
    }

    void onLeapFinished()
    {
        // Return to chasing state after leap finishes
        currentState = BossState.Chasing;
        leapStarted = false;
    }

    void onPunchFinished()
    {
        // Return to chasing state after punch finishes
        currentState = BossState.Chasing;
        punchTimer = punchCooldown;
    }

    float DistanceToPlayer()
    {
        // Return the distance to the player
        return Vector2.Distance(transform.position, player.position);
    }
    
    // Exception handling methods
    void HandleStuck()
    {
        // Check if boss is stuck
        float moved = Vector2.Distance(rb.position, lastPosition);
        if (moved < minMoveThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        // Update last position
        lastPosition = rb.position;

        // If stuck for too long, handle exception
        if (stuckTimer >= stuckTimeLimit)
        {
            HandleException();
            stuckTimer = 0f;
        }
    }

    void HandleException()
    {
        // Reset boss state and position to handle exception
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        // Teleport boss to player
        currentState = BossState.TeleportPunching;
    }
}
