using UnityEngine;
using UnityEngine.AI;

public class SoldierAttributes : MonoBehaviour
{
    // Base stats
    public float baseHealth = 10f;
    public float baseDamage = 3f;
    public float baseSpeed = 10f;
    public float baseRange = 50f;
    public float baseMorale = 100f;

    // Random stats for a bit of variation

    // Actual randomized stats
    [HideInInspector] public float health;
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float range;
    [HideInInspector] public float morale;

    NavMeshAgent agent;

    private void Start()
    {
        // Randomize stats within a range
        health = baseHealth * Random.Range(0.9f, 1.1f); // ±10% variation
        damage = baseDamage * Random.Range(0.9f, 1.1f); // ±10% variation
        speed = baseSpeed * Random.Range(0.9f, 1.1f);   // ±10% variation
        range = baseRange * Random.Range(0.9f, 1.1f);   // ±10% variation
        morale = baseMorale * Random.Range(0.9f, 1.1f); // ±10% variation

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }

        Debug.Log($"{gameObject.name} Stats - Health: {health}, Damage: {damage}, Speed: {speed}, Range: {range}, Morale: {morale}");
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetMorale()
    {
        return morale;
    }
}
