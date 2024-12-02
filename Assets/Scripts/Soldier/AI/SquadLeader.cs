using UnityEngine;
using UnityEngine.AI;

public class SquadLeader : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private Health health;
    private bool dead = false;

    private void Update()
    {
        int childCount = gameObject.transform.childCount;
        health = gameObject.GetComponent<Health>();
        dead = health.isDead;
        if (childCount <= 1 && dead)
        {
            Destroy(gameObject);
        }
    }

    public void setPosition(Vector3 offset, Transform leader) 
    {
        // Calculate the target position in the formation
        Vector3 targetPosition = leader.position + leader.TransformDirection(offset);
        // Move the follower to the target position using NavMesh
        navMeshAgent.SetDestination(targetPosition);
        
    }
}
