using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // TODO: Add respawn, ragdoll, disable player, etc.
        Destroy(gameObject); // Just for testing
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}