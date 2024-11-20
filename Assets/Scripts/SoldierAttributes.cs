
using UnityEngine;
using UnityEngine.AI;

public class SoldierAttributes : MonoBehaviour
{
    public float health = 10f;
    public float damage = 3f;
    public float speed = 10f;
    public float range = 50f;

    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

    }
}
