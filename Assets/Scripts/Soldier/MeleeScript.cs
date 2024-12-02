using UnityEngine;

public class MeleeAttackScript : MonoBehaviour
{
    public float attackDamage = 15f;
    public float attackRate = 1f;

    private float lastAttackTime = 0f;

    public void Attack(Transform target)
    {
        if (target == null) return;

        // Ensure we attack only at intervals
        if (Time.time - lastAttackTime >= attackRate)
        {
            lastAttackTime = Time.time;

            // Apply damage to the target
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage, gameObject); // Pass the attacker
                Debug.Log($"{gameObject.name} dealt {attackDamage} melee damage to {target.name}!");
            }
        }
    }
}
