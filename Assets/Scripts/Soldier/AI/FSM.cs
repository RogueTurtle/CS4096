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
    public float idleToWanderTime = 3f;

    private float idleTimer = 0f;
    private float wanderTimer = 0f;
    private float wanderCooldown = 3f;
    private float attackCooldown = 1f;
    private float attackTimer = 0f;

    private Health healthComponent;
    private GunScript gunScript;
    private SoldierAttributes attributes;

    public bool enableDebugging = true;

    // Shared focus fire target
    private static Transform FocusedTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        if (enemy == null)
        {
            enemy = FindClosestEnemy();
            if (enemy == null)
            {
                currentState = State.Idle;
                return;
            }
        }

        agent.SetDestination(enemy.position);

        if (Vector3.Distance(transform.position, enemy.position) <= attributes.range)
        {
            currentState = State.Attack;
        }
    }

    private void AttackState()
    {
        if (enemy == null || enemy.GetComponent<Health>() == null || enemy.GetComponent<Health>().IsRagdolling || enemy.GetComponent<Health>().health <= 0)
        {
            enemy = FindClosestEnemy();
            if (enemy == null)
            {
                agent.isStopped = false;
                currentState = State.Idle;
                return;
            }
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
        }

        if (attributes.health < attributes.GetHealth() * 0.2f)
        {
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
}
