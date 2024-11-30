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
    private float attackCooldown = 1f;
    private float attackTimer = 0f;

    // References to other components
    private Health healthComponent;
    private GunScript gunScript;
    private SoldierAttributes attributes;

    public bool enableDebugging = true; // Toggle for enabling/disabling debugging

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            LogError("NavMeshAgent component is missing!");
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
        Log($"Current State: {currentState}", false, "cyan");

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
        Log("Idle...", false, "green");

        idleTimer += Time.deltaTime;

        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            Log("Enemy detected! Switching to Chase.", false, "yellow");
            currentState = State.Chase;
            return;
        }

        if (idleTimer >= idleToWanderTime)
        {
            Log("Idle time exceeded. Switching to Wandering.", false, "green");
            idleTimer = 0f;
            currentState = State.Wandering;
        }
    }

    private void WanderingState()
    {
        Log("Wandering...", false, "green");

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderCooldown)
        {
            wanderTimer = 0f;
            wanderCooldown = Random.Range(1f, 4f);

            float randomRadius = Random.Range(5f, 15f);
            Vector3 randomDirection = Random.insideUnitSphere * randomRadius + transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, randomRadius, NavMesh.AllAreas))
            {
                Log($"Wandering to: {hit.position} (radius: {randomRadius})", true, "green");
                agent.SetDestination(hit.position);
            }
            else
            {
                LogWarning("Failed to find a valid random position for wandering.", "orange");
            }
        }

        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            Log("Enemy detected while wandering! Switching to Chase.", false, "yellow");
            currentState = State.Chase;
        }
    }

    private void ChaseState()
    {
        Log("Chasing...", false, "yellow");

        if (enemy == null)
        {
            LogWarning("Enemy is missing! Switching to Idle.", "orange");
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(enemy.position);

        if (Vector3.Distance(transform.position, enemy.position) <= attributes.range)
        {
            Log("Enemy in attack range! Switching to Attack.", false, "red");
            currentState = State.Attack;
        }
    }

    private void AttackState()
    {
        Log("Attacking...", false, "red");

        if (enemy == null)
        {
            LogWarning("Enemy is missing! Switching to Idle.", "orange");
            currentState = State.Idle;
            agent.isStopped = false;
            return;
        }

        agent.isStopped = true;

        Vector3 direction = (enemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            gunScript.Shoot();
            Log("Attacking enemy!", false, "red");
        }

        if (attributes.health < attributes.GetHealth() * 0.2f)
        {
            Log("Health is too low! Switching to Retreat.", false, "blue");
            agent.isStopped = false;
            currentState = State.Retreat;
        }
    }

    private void RetreatState()
    {
        Log("Retreating...", false, "blue");

        if (retreatPoint == null)
        {
            LogError("No retreat point assigned! Switching to Idle.", "magenta");
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(retreatPoint.position);

        if (Vector3.Distance(transform.position, retreatPoint.position) < 2f)
        {
            Log("Reached retreat point. Switching to Idle.", false, "blue");
            currentState = State.Idle;
        }
    }

    // Debugging Helpers
    private void Log(string message, bool limitFrequency = false, string color = "white")
    {
        if (!enableDebugging) return;

        Debug.Log($"<color={color}>{gameObject.name}: {message}</color>");
    }

    private void LogWarning(string message, string color = "orange")
    {
        if (!enableDebugging) return;

        Debug.LogWarning($"<color={color}>{gameObject.name}: {message}</color>");
    }

    private void LogError(string message, string color = "magenta")
    {
        if (!enableDebugging) return;

        Debug.LogError($"<color={color}>{gameObject.name}: {message}</color>");
    }
}
