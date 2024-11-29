using UnityEngine;
using UnityEngine.AI;

public class TempAIFSM : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Retreat }
    public State currentState = State.Idle;

    public Transform enemy; 
    public Transform retreatPoint; //IF we decide to have retreating enabled
    public NavMeshAgent agent; 

    //All of these are placeholder values
    public float detectionRange = 10f; // Distance to detect enemies
    public float attackRange = 2f; // Distance to attack
    public float retreatThreshold = 20f; // Health threshold to retreat
    public float health = 100f; // Character's health
    public float attackDamage = 10f; // Damage dealt to enemy per attack

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Evaluate the current state
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
        // Look for an enemy within detection range
        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            currentState = State.Chase; // Transition to Chase state
        }
    }

    private void ChaseState()
    {
        Debug.Log("Chasing...");
        if (enemy == null)
        {
            currentState = State.Idle; // Transition back to Idle if no enemy
            return;
        }

        // Set destination to the enemy's position
        agent.SetDestination(enemy.position);

        // Transition to Attack if within range
        if (Vector3.Distance(transform.position, enemy.position) <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    private void AttackState()
    {
        Debug.Log("Attacking...");
        if (enemy == null)
        {
            currentState = State.Idle; // Transition to Idle if enemy is gone
            return;
        }

        // Simulate attacking the enemy (e.g., reduce enemy health)
        Debug.Log("Enemy hit!");

        // Transition to Retreat if health is low
        if (health < retreatThreshold)
        {
            currentState = State.Retreat;
        }
    }

    private void RetreatState()
    {
        Debug.Log("Retreating...");
        agent.SetDestination(retreatPoint.position); // Move to retreat point

        // Transition to Idle if reached the retreat point
        if (Vector3.Distance(transform.position, retreatPoint.position) < 2f)
        {
            currentState = State.Idle;
        }
    }
}
