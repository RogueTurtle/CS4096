using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Initial health
    private RagdollController ragdollController;

    private void Start()
    {
        // Get reference to the RagdollController
        ragdollController = GetComponent<RagdollController>();
    }

    public void TakeDamage(float damage)
    {
        // Reduce health
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {health}");

        // Check if health is depleted
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} is dead!");

        // Activate the "ragdoll" physics
        if (ragdollController != null)
        {
            ragdollController.ActivateRagdoll();
        }

        // Optionally destroy the object after a delay
        Destroy(gameObject, 5f);
    }
}
