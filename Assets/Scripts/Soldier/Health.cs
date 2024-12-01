using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Initial health
    public float destroyDelay = 5f; // Time to destroy the object after death
    private RagdollController ragdollController;
    private bool isRagdolling = false; // Ragdoll status

    private void Start()
    {
        // Get reference to the RagdollController
        ragdollController = GetComponent<RagdollController>();
    }

    public bool IsRagdolling
    {
        get { return isRagdolling; }
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
        isRagdolling = true; // Set ragdolling status

        // Activate the "ragdoll" physics
        if (ragdollController != null)
        {
            ragdollController.ActivateRagdoll();
        }

        // Destroy the object after a delay
        Destroy(gameObject, destroyDelay);
    }
}
