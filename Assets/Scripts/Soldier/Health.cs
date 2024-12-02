using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f; // Initial health
    public float destroyDelay = 5f; // Time to destroy the object after death
    private RagdollController ragdollController;
    private bool isRagdolling = false; // Ragdoll status
    private float fullHealth;
    private bool shouldRetreat = false;

    // Property to check if the unit is dead
    public bool IsDead => health <= 0;

    public bool IsRagdolling => isRagdolling;

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
        fullHealth = health;
    }

    public void TakeDamage(float damage, GameObject attacker = null)
    {
        if (IsDead) return; // Prevent further damage to a dead unit

        // Reduce health
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {health}");
        if ((fullHealth / 3) > health)
        {
            shouldRetreat = true;
        }
        // Check if health is depleted
        if (IsDead)
        {
            Die(attacker);
        }
    }

    public bool Retreat()
    {
        return shouldRetreat;
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
        // Optional: Increment kill counters or other gameplay logic
    }
}
