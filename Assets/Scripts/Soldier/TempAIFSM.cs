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

    // References to other components
    private Health healthComponent;
    private GunScript gunScript;
    private SoldierAttributes attributes;

    private void Start()
    {
        // Assign NavMeshAgent and check for its existence
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }

        // Assign references to other components
        healthComponent = GetComponent<Health>();
        gunScript = GetComponent<GunScript>();
        attributes = GetComponent<SoldierAttributes>();

        // Set agent speed from SoldierAttributes
        if (attributes != null && agent != null)
        {
            agent.speed = attributes.GetSpeed();
        }
    }

    private void Update()
    {
        Debug.Log($"Current State: {currentState}");

        // FSM logic
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

        if (Vector3.Distance(transform.position, enemy.position) <= attributes.range)
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

        // Use GunScript to shoot
        gunScript.Shoot();

        // Check for health threshold to retreat
        if (attributes.health < attributes.GetHealth() * 0.2f) // Retreat if health < 20%
        {
            Debug.Log("Health is too low! Switching to Retreat.");
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
