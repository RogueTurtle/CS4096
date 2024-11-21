
using UnityEngine;
using UnityEngine.AI;

public class SoldierAttributes : MonoBehaviour
{
    public float health = 10f;
    public float damage = 3f;
    public float speed = 10f;
    public float range = 50f;
    public float morale = 100f;

    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
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
