using UnityEngine;
using UnityEngine.AI;

public class TempAIFSM : MonoBehaviour
{
    public enum State { Idle, Wandering, Chase, Attack, Retreat }
    public State currentState = State.Idle;

    public Transform enemy;
    public Transform retreatPoint;
    public NavMeshAgent agent;

    public float detectionRange = 10f;
    public float idleToWanderTime = 3f; // Time to wait in Idle before switching to Wandering

    private float idleTimer = 0f; // Timer for idle to wander
    private float wanderTimer = 0f; // Timer for wandering
    private float wanderCooldown = 3f; // Default cooldown for wandering

    // References to other components
    private Health healthComponent;
    private GunScript gunScript;
    private SoldierAttributes attributes;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }

        healthComponent = GetComponent<Health>();
        gunScript = GetComponent<GunScript>();
        attributes = GetComponent<SoldierAttributes>();

        if (attributes != null && agent != null)
        {
            agent.speed = attributes.speed;
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
            case State.Wandering:
                WanderingState();
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

        idleTimer += Time.deltaTime;

        // Check for enemies
        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            Debug.Log("Enemy detected! Switching to Chase.");
            currentState = State.Chase;
            return;
        }

        // Transition to Wandering after idle time
        if (idleTimer >= idleToWanderTime)
        {
            Debug.Log("Idle time exceeded. Switching to Wandering.");
            idleTimer = 0f; // Reset the timer
            currentState = State.Wandering;
        }
    }

    private void WanderingState()
{
    Debug.Log("Wandering...");

    wanderTimer += Time.deltaTime;

    // Randomize the time between movements
    if (wanderTimer >= wanderCooldown)
    {
        wanderTimer = 0f; // Reset the timer
        wanderCooldown = Random.Range(1f, 4f); // Randomize cooldown between 1 and 4 seconds

        // Generate a random position within a random radius
        float randomRadius = Random.Range(5f, 15f); // Randomize distance between 5 and 15 units
        Vector3 randomDirection = Random.insideUnitSphere * randomRadius; // Use the random radius
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomRadius, NavMesh.AllAreas))
        {
            Debug.Log($"Wandering to: {hit.position} (radius: {randomRadius})");
            agent.SetDestination(hit.position); // Move to the new random position
        }
        else
        {
            Debug.LogWarning("Failed to find a valid random position for wandering.");
        }
    }

    // Check for nearby enemies while wandering
    if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
    {
        Debug.Log("Enemy detected while wandering! Switching to Chase.");
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

        gunScript.Shoot();

        if (attributes.health < attributes.GetHealth() * 0.2f)
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
