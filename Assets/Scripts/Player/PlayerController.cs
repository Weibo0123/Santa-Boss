using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement parameters
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;     
    [SerializeField] float jumpForce = 10f;     
    [SerializeField] Transform groundCheck;     
    [SerializeField] float checkRadius = 0.2f; 
    [SerializeField] LayerMask Ground;
    [Header("Knockback Control")]   
    [SerializeField] float controlMultiplier = 1f;
    [SerializeField] float controlRecoverTime = 0.2f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping;
    // Player states
    enum PlayerState { Normal, Knockback }
    PlayerState currentState = PlayerState.Normal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = 0;


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }
    }

    void FixedUpdate()
    {
        // Handle movement based on current state
        switch (currentState)
        {
            case PlayerState.Normal:
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * controlMultiplier, rb.linearVelocity.y);
                if (isJumping)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    isJumping = false;
                }
                break;
            case PlayerState.Knockback:
                // During knockback, normal movement is disabled
                break;

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
