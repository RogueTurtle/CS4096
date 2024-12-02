using UnityEngine;
using UnityEngine.AI;

public class TempAIFSM : MonoBehaviour
{
    public enum State { Idle, Wandering, Chase, Attack, Retreat }
    public State currentState = State.Idle;

    public State GetCurrentState()
    {
        return currentState;
    }

    public Transform enemy;
    public Transform retreatPoint;
    public NavMeshAgent agent;

    public float detectionRange = 10f;
    public float idleToWanderTime = 3f;

    private float idleTimer = 0f;
    private float wanderTimer = 0f;
    private float wanderCooldown = 3f;
    private float attackCooldown = 1f;
    private float attackTimer = 0f;

    private Health healthComponent;
    private GunScript gunScript;
    private SoldierAttributes attributes;
    private Health health;

    public bool enableDebugging = true;

    // Shared focus fire target
    private static Transform FocusedTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<Health>();
        gunScript = GetComponent<GunScript>();
        attributes = GetComponent<SoldierAttributes>();
        health = GetComponent<Health>();

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
        idleTimer += Time.deltaTime;

        if (enemy == null)
        {
            enemy = FindClosestEnemy();
        }

        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            currentState = State.Chase;
            return;
        }

        if (idleTimer >= idleToWanderTime)
        {
            idleTimer = 0f;
            currentState = State.Wandering;
        }
    }

    private void WanderingState()
    {
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
                agent.SetDestination(hit.position);
            }
        }

        if (enemy == null)
        {
            enemy = FindClosestEnemy();
        }

        if (enemy != null && Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            currentState = State.Chase;
        }
    }

   private void ChaseState()
{
    Log("Chasing...", false, "yellow");

    // Continuously validate the enemy
    if (enemy == null || enemy.GetComponent<Health>() == null || enemy.GetComponent<Health>().health <= 0)
    {
        LogWarning("Current enemy is invalid or dead. Finding a new target.", "orange");
        enemy = FindClosestEnemy();
        if (enemy == null)
        {
            Log("No valid enemies found. Switching to Idle.", false, "orange");
            currentState = State.Idle;
            return;
        }
    }

    // Update the agent's destination to the enemy's current position
    agent.SetDestination(enemy.position);

    // Check if the enemy is within attack range
    float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
    if (distanceToEnemy <= attributes.range)
    {
        Log("Enemy in range! Switching to Attack.", false, "red");
        currentState = State.Attack;
    }
    else
    {
        Log($"Chasing enemy at {enemy.position}. Distance: {distanceToEnemy}", false, "yellow");
    }
}


private void AttackState()
{
    Log("Attacking...", false, "red");

    // Validate the enemy
    if (enemy == null || enemy.GetComponent<Health>() == null || enemy.GetComponent<Health>().health <= 0)
    {
        LogWarning("Enemy is invalid or dead. Searching for a new target.", "orange");
        enemy = FindClosestEnemy();
        if (enemy == null)
        {
            Log("No valid enemies found. Switching to Idle.", false, "orange");
            agent.isStopped = false;
            currentState = State.Idle;
            return;
        }
    }

    float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);

    // If the unit is a melee unit
    if (GetComponent<MeleeAttackScript>() != null)
    {
        // If out of melee range, move toward the enemy
        if (distanceToEnemy > attributes.range)
        {
            Log($"Enemy out of melee range. Moving closer. Distance: {distanceToEnemy}", false, "yellow");
            agent.isStopped = false; // Ensure movement is enabled
            agent.SetDestination(enemy.position); // Update destination to follow the enemy
        }
        else
        {
            // If within range, stop moving and attack
            Log($"Enemy in melee range. Attacking now. Distance: {distanceToEnemy}", false, "red");
            agent.isStopped = true; // Stop movement
            GetComponent<MeleeAttackScript>().Attack(enemy); // Perform melee attack
        }
    }
    else if (gunScript != null) // For ranged units
    {
        agent.isStopped = true; // Stop moving for ranged units
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            gunScript.Shoot();
            Log("Attacking enemy with ranged weapon!", false, "red");
        }
    }

    // Retreat if health is low
    if (health.Retreat())
    {
        Log("Health is too low! Switching to Retreat.", false, "blue");
        agent.isStopped = false;
        currentState = State.Retreat;
    }
}

    private void RetreatState()
    {
        if (retreatPoint == null)
        {
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(retreatPoint.position);

        if (Vector3.Distance(transform.position, retreatPoint.position) < 2f)
        {
            currentState = State.Idle;
        }
    }

    private Transform FindClosestEnemy()
    {
        string opposingTeamTag = gameObject.tag == "Team1" ? "Team2" : "Team1";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(opposingTeamTag);

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        if (ShouldFocusFire() && FocusedTarget != null && !FocusedTarget.GetComponent<Health>().IsRagdolling)
        {
            return FocusedTarget;
        }

        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj.GetComponent<Health>()?.IsRagdolling == true) continue;

            float distance = Vector3.Distance(transform.position, enemyObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyObj.transform;
            }
        }

        if (closestEnemy != null && Random.value < 0.3f) // 30% chance to focus fire
        {
            FocusedTarget = closestEnemy;
        }

        return closestEnemy;
    }

    private bool ShouldFocusFire()
    {
        return Random.value < 0.5f; // 50% chance to attempt focus fire
    }

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
