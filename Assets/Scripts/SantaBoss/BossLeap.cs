using UnityEngine;
using System.Collections;

public class BossLeap : MonoBehaviour
{
    // Leap parameters
    Animator animator;
    [Header("Leap")]
    [SerializeField] float leapForceY = 15f;
    [SerializeField] float leapForceX = 8f;
    [SerializeField] float leapCooldown = 3f;
    [SerializeField] float leapWindupTime = 0.2f;
    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    public System.Action onLeapFinished;
    Rigidbody2D rb;

    // Check if boss is grounded
    bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    bool canLeap = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    public bool CanLeap()
    {
        return canLeap;
    }

    // Leap routine
    IEnumerator Leap(Transform player)
    {
        canLeap = false;
        animator.SetBool("isRunning", false);
        animator.SetBool("isLeapingUp", false);
        animator.SetBool("isLeapingDown", false);
        yield return new WaitForSeconds(leapWindupTime);
        animator.SetBool("isLeapingUp", true);
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(dir * leapForceX, leapForceY);
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        animator.SetBool("isLeapingUp", false);
        animator.SetBool("isLeapingDown", true);
        yield return new WaitUntil(() => IsGrounded());
        animator.SetBool("isLeapingDown", false);
        onLeapFinished?.Invoke();
        yield return new WaitForSeconds(leapCooldown);
        canLeap = true;
    }

    // Start leap attack
    public void leap(Transform player)
    {
        if (canLeap)
        {
            StartCoroutine(Leap(player));
        }
    }

}
