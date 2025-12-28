using UnityEngine;

public class SantaBoss : MonoBehaviour
{
    public Transform player;

    public float moveSpeed;
    public float chaseRange = 8f;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D bossCollider;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		animator =  GetComponent<Animator>();

        bossCollider = GetComponent<Collider2D>();
        playerCollider = player.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    void ChasePlayer()
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

    void StopChasing()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isRunning", false);
    }
}