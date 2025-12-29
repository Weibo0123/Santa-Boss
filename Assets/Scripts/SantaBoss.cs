using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    enum BossState
    {
        Idle,
        Chasing,
        Leaping
    }
    private BossState currentState = BossState.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		animator =  GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdle();
                CheckJumpConditions(Mathf.Sign(player.position.x - transform.position.x));
                SwitchToChasing();
                break;
            case BossState.Chasing:
                HandleChasing();
                CheckJumpConditions(Mathf.Sign(player.position.x - transform.position.x));
                SwitchToIdle();
                SwitchToLeaping();
                break;
            case BossState.Leaping:
                HandleLeaping();
                currentState = BossState.Chasing;
                break;
        }
    }

    public float moveSpeed;
    public float chaseRange = 8f;
    void HandleChasing()
    {
        float directionX = Mathf.Sign(player.position.x - transform.position.x);

        transform.position = new Vector3(
            transform.position.x + directionX * moveSpeed * Time.deltaTime,
            transform.position.y,
            transform.position.z
        );

        if (animator != null)
        {
            animator.SetBool("isRunning", true);
        }
        if (spriteRenderer != null)
        {
            if (directionX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (directionX < 0){
                spriteRenderer.flipX = true;
            }
        }
    }

    [SerializeField] float gapCheckDistance = 0.5f;
    [SerializeField] LayerMask Ground; 
    
    bool IsGapAhead(float dir)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * dir * 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, gapCheckDistance, Ground);
        return hit.collider == null;
    }

    [SerializeField] float obstacleCheckDistance = 0.5f;
    bool IsObstacleAhead(float dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * dir, obstacleCheckDistance, Ground);
        return hit.collider != null;
    }

    bool isPlayerAhead(float dir)
    {
        float toPlayerX = player.position.x - transform.position.x;
        return Mathf.Sign(toPlayerX) == Mathf.Sign(dir);
    }
    [SerializeField] Transform GroundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;

    bool isGrounded()
    {
       return Physics2D.Raycast(GroundCheck.position, Vector2.down, groundCheckRadius, Ground);
    }
    void CheckJumpConditions(float dir)
    {
        if (isGrounded() && isPlayerAhead(dir) && (IsObstacleAhead(dir) || IsGapAhead(dir)))
        {
            Jump();
        }
    }
    [SerializeField] float jumpForce = 10f;
    public bool isJumping;
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        isJumping = true;
    }
    void SwitchToChasing()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            currentState = BossState.Chasing;
        }
    }

    void SwitchToIdle()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange)
        {
            currentState = BossState.Idle;
        }
    }

    void SwitchToLeaping()
    {
        currentState = BossState.Leaping;
    }

    void HandleLeaping()
    {
        // Placeholder for leaping behavior
    }

    void HandleIdle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);
    }
}