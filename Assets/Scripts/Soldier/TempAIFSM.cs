using UnityEngine;
using UnityEngine.AI;

public class TempAIFSM : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Retreat }
    public State currentState = State.Idle;

    public Transform enemy; 
    public Transform retreatPoint; 
    public NavMeshAgent agent; 

    public float detectionRange = 10f; 
    public float attackRange = 2f; 
    public float retreatThreshold = 20f; 
    public float health = 100f; 
    public float attackDamage = 10f; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }

    private void Update()
    {
        Debug.Log($"Current State: {currentState}");

        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Retreat:
                RetreatState();
                break;
        }
    }

    private void IdleState()
    {
        Debug.Log("Idle...");

        if (enemy == null)
        {
            Debug.LogWarning("No enemy assigned!");
            return;
        }

        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
        Debug.Log($"Distance to enemy: {distanceToEnemy}");

        if (distanceToEnemy < detectionRange)
        {
            Debug.Log("Enemy detected! Switching to Chase.");
            currentState = State.Chase;
        }
    }

    private void ChaseState()
    {
        Debug.Log("Chasing...");

        if (enemy == null)
        {
            Debug.LogWarning("Enemy is missing! Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(enemy.position);

        if (Vector3.Distance(transform.position, enemy.position) <= attackRange)
        {
            Debug.Log("Enemy in attack range! Switching to Attack.");
            currentState = State.Attack;
        }
    }

    private void AttackState()
    {
        Debug.Log("Attacking...");

        if (enemy == null)
        {
            Debug.LogWarning("Enemy is missing! Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        Debug.Log("Enemy hit!");

        if (health < retreatThreshold)
        {
            Debug.Log("Health low! Switching to Retreat.");
            currentState = State.Retreat;
        }
    }

    private void RetreatState()
    {
        Debug.Log("Retreating...");

        if (retreatPoint == null)
        {
            Debug.LogError("No retreat point assigned! Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(retreatPoint.position);

        if (Vector3.Distance(transform.position, retreatPoint.position) < 2f)
        {
            Debug.Log("Reached retreat point. Switching to Idle.");
            currentState = State.Idle;
        }
    }
}
