using UnityEngine;
using System.Collections;

public class BossPunch : MonoBehaviour
{
    [Header("Punch")]  
    [SerializeField] float windUpTime = 0.5f;
    [SerializeField] float teleportOffset = 1.2f;
    public GameObject punchHitbox;
    public GameObject DamageHitbox;
    Animator animator;
    bool isPunching = false;
    public bool canPunch => !isPunching;
    Rigidbody2D rb;

    void Awake()
    {
        animator = GetComponent<Animator>();
        punchHitbox.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    public System.Action OnPunchFinished; 
    public void TryPunch(Transform player, System.Action onFinished)
    {
        if (canPunch)
        {
            isPunching = true;
            OnPunchFinished = onFinished;
            StartCoroutine(PunchRoutine(player, onFinished));
        }
    }

    IEnumerator PunchRoutine(Transform player, System.Action onFinished)
    {
        DamageHitbox.SetActive(false);
        animator.SetBool("isRunning", false);
        // Wind-up
        yield return new WaitForSeconds(windUpTime);

        // Teleport
        float side = Mathf.Sign(player.position.x - transform.position.x);
        Vector3 newPosition = new Vector3(player.position.x + side * teleportOffset, player.position.y, transform.position.z);
        transform.position = newPosition;
        if (animator != null) animator.transform.localScale = new Vector3(-side, 1, 1);
        rb.simulated = true;
        // Start punch
        animator.SetTrigger("punch");
    }

    public void StartPunchHitbox()
    {
        punchHitbox.SetActive(true);
    }

    public void EndPunchHitbox()
    {
        punchHitbox.SetActive(false);
        DamageHitbox.SetActive(true);
    }

    public void FinishPunch()
    {
        isPunching = false;
        OnPunchFinished?.Invoke();
    }

}
