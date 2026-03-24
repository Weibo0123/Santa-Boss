using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // Health parameters
    [SerializeField] int maxHealth = 100;
    [SerializeField] float InvincibilityCooldown = 1f;
    [SerializeField] float knockbackDuration = 0.3f;

#if UNITY_EDITOR
    public UnityEditor.SceneAsset sceneAsset; // Drag your scene here in the Inspector
#endif

    [HideInInspector]
    public string sceneName;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name; // Automatically get the scene name
#endif
    }


    int currentHealth;
    bool isInvincible = false;
    bool isDead = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Check if Space is pressed
        if (isDead)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName); // Load the specific scene
            }
            else
            {
                Debug.LogWarning("Scene not assigned in the Inspector!");
            }
        }
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

    // Handle player death
    public void Die()
    {
        // Handle player death (e.g., reload scene, show game over screen)
        Debug.Log("Player Died");
        isDead = true;
    }

}

