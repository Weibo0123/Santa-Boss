using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement parameters
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;     
    [SerializeField] float acceleration = 30f;     
    [SerializeField] float GroundDeceleration = 20f;
    [SerializeField] float AirDeceleration = 15f;
    [Header("Jump")]
    [SerializeField] LayerMask Ground;
    [SerializeField] float jumpForce = 15f;     
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius = 0.2f; 
    [SerializeField] float jumpBuffer = 0.3f;
    [SerializeField] float coyoteTime = 0.3f;
    [Header("Gravity")]
    [SerializeField] float gravityScale = 3f;
    [SerializeField] float fallMultiplier = 2f;
    [SerializeField] float jumpCutMultiplier = 1f;
    [SerializeField] float jumpCutSmooth = 30f;
    [Header("Knockback Control")]   
    [SerializeField] float controlMultiplier = 1f;
    [SerializeField] float controlRecoverTime = 0.2f;
    Vector2 moveInput;
    bool isGrounded;
    bool isJumping;
    float jumpBufferTimer;
    float coyoteTimer;
    bool jumpHeld;
    // Player states
    enum PlayerState { Normal, Knockback }
    PlayerState currentState = PlayerState.Normal;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = 0;
        jumpHeld = Input.GetButton("Jump");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBuffer;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Handle movement based on current state
        switch (currentState)
        {
            case PlayerState.Normal:
                float targetSpeed = moveInput.x * moveSpeed;
                float accelRate;
                if (Mathf.Abs(targetSpeed) > 0.01f)
                {
                    accelRate = acceleration;
                }
                else
                {
                    accelRate = isGrounded ? GroundDeceleration : AirDeceleration;
                }
                float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
                if (moveInput.x > 0) {
                    spriteRenderer.flipX = false;
                    animator.SetBool("isRunning", true);
                }
                else if (moveInput.x < 0) 
                {
                    spriteRenderer.flipX = true;
                    animator.SetBool("isRunning", true);
                }
                else 
                {
                    animator.SetBool("isRunning", false);
                }
                if (jumpBufferTimer > 0f && coyoteTimer > 0f)
                {
                    animator.SetBool("isJumping", true);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    jumpBufferTimer = 0f;
                    coyoteTimer = 0f;
                    isJumping = true;
                }
                HandleGravity();
                if (isGrounded && rb.linearVelocity.y <= 0)
                {
                    animator.SetBool("isJumping", false);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    isJumping = false;
                }
                break;
            case PlayerState.Knockback:
                // During knockback, normal movement is disabled
                break;

        }
    }

    // Handle gravity and jump cut
    void HandleGravity()
    {   
        float velY = rb.linearVelocity.y;
        if (velY > 0)
        {
            if (!jumpHeld)
            {
                float targetY = velY * jumpCutMultiplier;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.MoveTowards(velY, targetY, jumpCutSmooth * Time.fixedDeltaTime));
            }
        }
        else if (velY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else
        {
            isJumping = false;
        }
    }
    // Apply knockback to player
    public void ApplyKnockback(Vector2 force, float duration)
    {
        Debug.Log("Applying Knockback");
        if (currentState == PlayerState.Knockback) return;

        currentState = PlayerState.Knockback;
        StartCoroutine(KnockbackCoroutine(force, duration));
    }

    IEnumerator KnockbackCoroutine(Vector2 force, float duration)
    {
        controlMultiplier = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        currentState = PlayerState.Normal;
        float t = 0f;
        while (t < controlRecoverTime)
        {
            t += Time.deltaTime;
            controlMultiplier = Mathf.Lerp(0f, 1f, t / controlRecoverTime);
            yield return null;
        }
    }
}
