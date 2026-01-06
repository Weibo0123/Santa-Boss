using UnityEngine;

public class BossDamage : MonoBehaviour
{
    // Damage parameters
    [Header("Damage")]
    [SerializeField] int damageAmount = 1;
    [SerializeField] float knockbackForceX = 7f;
    [SerializeField] float knockbackForceY = 5f;
    
    // Detect collision with player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;   
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        Debug.Log("BossDamage collided with " + other.name);
        // Apply knockback
        float knockbackDirection = other.transform.position.x < transform.position.x ? -1 : 1;
        Vector2 knockback = new Vector2(knockbackForceX * knockbackDirection, knockbackForceY);        
        playerHealth.TakeDamage(damageAmount, knockback);
    }
}
