using UnityEngine;

public class BossMovement : MonoBehaviour
{
    // Movement parameters
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float horizontalDeadZone = 0.2f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 10f;
    public bool isJumping = false;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Obstacle Check")]
    [SerializeField] float gapCheckDistance = 2f;
    [SerializeField] float obstacleCheckDistance = 2f;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Chase the player
    public void Chase(Transform player)
    {
        float dx = player.position.x - transform.position.x;
        if (Mathf.Abs(dx) < horizontalDeadZone)
        {
            Stop();
            return;
        }
        float dir = Mathf.Sign(dx);

        transform.position += new Vector3(
            dir * moveSpeed * Time.fixedDeltaTime,
            0,
            0
        );

        animator.SetBool("isRunning", true);
        animator.transform.localScale = new Vector3(dir, 1, 1);
    }

    // Stop movement
    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);
    }

    // Try to jump over obstacles or gaps
    public void TryJump(Transform player)
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        if (!IsGrounded()) return;
        if (!IsPlayerAhead(player, dir)) return;

        if (IsGapAhead(dir) || IsObstacleAhead(dir))
        {
            Jump();
        }
    }

    // Perform jump
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        isJumping = true;
    }

    // Check if grounded
    public bool IsGrounded()
    {
        return Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    // Check for gaps ahead
    bool IsGapAhead(float dir)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * dir * 0.6f;
        return !Physics2D.Raycast(origin, Vector2.down, gapCheckDistance, groundLayer);
    }

    // Check for obstacles ahead
    bool IsObstacleAhead(float dir)
    {
        return Physics2D.Raycast(
            transform.position,
            Vector2.right * dir,
            obstacleCheckDistance,
            groundLayer
        );
    }

    // Check if player is ahead in movement direction
    bool IsPlayerAhead(Transform player, float dir)
    {
        return Mathf.Sign(player.position.x - transform.position.x) == dir;
    }
}
