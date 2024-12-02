using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Initial health
    public float destroyDelay = 5f; // Time to destroy the object after death
    private RagdollController ragdollController;
    private bool isRagdolling = false; // Ragdoll status
    public bool IsDead => health <= 0;


    private void Start()
    {
        // Get reference to the RagdollController
        ragdollController = GetComponent<RagdollController>();

        // Sync health with randomized health from SoldierAttributes
        var attributes = GetComponent<SoldierAttributes>();
        if (attributes != null)
        {
            health = attributes.health; // Use the randomized health
        }
    }

    public bool IsRagdolling => isRagdolling;

    public void TakeDamage(float damage, GameObject attacker = null)
    {
        // Reduce health
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {health}");

        // Check if health is depleted
        if (health <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject attacker)
    {
        Debug.Log($"{gameObject.name} has died!");
        isRagdolling = true; // Mark as ragdoll

        // Notify the attacker using OnKill
        if (attacker != null)
        {
            var attackerAttributes = attacker.GetComponent<SoldierAttributes>();
            if (attackerAttributes != null)
            {
                OnKill(attackerAttributes);
            }
        }

        // Activate the "ragdoll" physics
        if (ragdollController != null)
        {
            ragdollController.ActivateRagdoll();
        }

        // Destroy the object after a delay
        Destroy(gameObject, destroyDelay);
    }

    private void OnKill(SoldierAttributes attackerAttributes)
    {
        Debug.Log($"{attackerAttributes.gameObject.name} killed {gameObject.name}!");
    }
}