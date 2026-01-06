using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // Health parameters
    [SerializeField] int maxHealth = 100;
    [SerializeField] float InvincibilityCooldown = 1f;
    [SerializeField] float knockbackDuration = 0.3f;

    int currentHealth;
    bool isInvincible = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    // Apply damage to player
    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplyKnockback(knockback, knockbackDuration);
            }
        }
    }

    // Invincibility routine
    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityCooldown);
        isInvincible = false;
    }

    void Die()
    {
        // Handle player death (e.g., reload scene, show game over screen)
        Debug.Log("Player Died");
    }
}

