
using UnityEngine;
using UnityEngine.AI;

public class FollowLeader : MonoBehaviour
{
    private TempAIFSM leaderFSM;
    private TempAIFSM.State lastLeaderState;

    public Transform leader;

    public float followDistance = 3f;

    public bool dead = false;

    private Health health;

    private NavMeshAgent navMeshAgent;
    //public Vector3 offset;
    public Vector3[] wanderingFormation = new Vector3[] {
        new Vector3(-2, 0, 2),   // Top-left soldier
        new Vector3(2, 0, 2),    // Top-right soldier
        new Vector3(-2, 0, -2),  // Bottom-left soldier
        new Vector3(2, 0, -2)    // Bottom-right soldier
    };
    public Vector3[] attackFormation = new Vector3[] {
        new Vector3(-20, 0, -10),  // Left soldier, further behind and to the left
        new Vector3(-10, 0, -20),  // Second left soldier, behind and to the left
        new Vector3(10, 0, -20),   // Second right soldier, behind and to the right
        new Vector3(20, 0, -10)    // Right soldier, further behind and to the right
    };
    Vector3[] idleFormation = new Vector3[]
    {
        new Vector3(-5, 0, -5), // First soldier: Left and slightly back
        new Vector3(5, 0, 5),  // Second soldier: Right and slighlty forward
        new Vector3(-10, 0, -5), // Third soldier: Further Left and slighlty back
        new Vector3(10, 0, 5)   // Fourth soldier: Further Right and slighlty forward
    };


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        leaderFSM = gameObject.GetComponent<TempAIFSM>();
        lastLeaderState = leaderFSM.currentState;
    }
    void Update()
    {
        health = GetComponent<Health>();
        dead = health.IsDead;
        if (leaderFSM == null)
        {
            Debug.LogWarning("Leader has no FSM script attached");    
        }
        
        TempAIFSM.State leaderState = leaderFSM.GetCurrentState(); 
        if (leaderFSM.currentState != lastLeaderState && !dead) {
            UpdateFormation(leaderFSM.currentState);
            lastLeaderState=leaderFSM.currentState;
        }
        else
        {
        
            setIdleFormation();
        }

    }

    private void UpdateFormation(TempAIFSM.State leaderState)
    {
        switch (leaderState) {
            case TempAIFSM.State.Idle:
                setIdleFormation();
                break;
            case TempAIFSM.State.Attack:
                setAttackFormation();
                break;
            case TempAIFSM.State.Wandering:
                setWanderingFormation();
                break;
            case TempAIFSM.State.Chase:
                setAttackFormation();
                break;
            case TempAIFSM.State.Retreat:
                setRetreatFormation();
                break;
            default:
                Debug.LogWarning("Unhandled leader state");
                break;

        }
    }

    private void setIdleFormation()
    {
        if (leader != null)
        {
            // Get the follower's assigned offset based on its position in the formation
            int index = transform.GetSiblingIndex();
            index -= 1;
            Vector3 offset = idleFormation[index];
            //Debug.Log(gameObject.name + ":" + index);

            // Calculate the target position in the formation
            Vector3 targetPosition = leader.position + leader.TransformDirection(offset);
            // Move the follower to the target position using NavMesh
            navMeshAgent.SetDestination(targetPosition);
            
        }
        else
        {
            // Stop moving when within the follow distance
            navMeshAgent.ResetPath();
        }
    }

    private void setAttackFormation()
    {
        if (leader != null)
        {
            // Get the follower's assigned offset based on its position in the formation
            int index = transform.GetSiblingIndex();
            index -= 1;
            Vector3 offset = attackFormation[index];
            Vector3 positionOffset = attackFormation[index];
            transform.position = leader.position - leader.forward * positionOffset.z + leader.right * positionOffset.x;
            //Debug.Log(gameObject.name + ":" + index);

            // Calculate the target position in the formation
            Vector3 targetPosition = leader.position + leader.TransformDirection(offset);
            // Move the follower to the target position using NavMesh
            navMeshAgent.SetDestination(targetPosition);
            
        }
        else
        {
            // Stop moving when within the follow distance
            navMeshAgent.ResetPath();
        }
    }
    private void setWanderingFormation()
    {
        if (leader != null)
        {
            // Get the follower's assigned offset based on its position in the formation
            int index = transform.GetSiblingIndex();
            index -= 1;
            Vector3 offset = wanderingFormation[index];
            //Debug.Log(gameObject.name + ":" + index);

            // Calculate the target position in the formation
            Vector3 targetPosition = leader.position + leader.TransformDirection(offset);
            // Move the follower to the target position using NavMesh
            navMeshAgent.SetDestination(targetPosition);
            
        }
        else
        {
            // Stop moving when within the follow distance
            navMeshAgent.ResetPath();
        }
    }
    private void setRetreatFormation()
    {
        if (leader != null)
        {
            // Calculate the desired follow position behind the leader
            Vector3 targetPosition = leader.position - leader.forward * followDistance;
            // Set the NavMeshAgent's destination to the follow position
            navMeshAgent.SetDestination(targetPosition);
            
        }
        else
        {
            // Stop moving when within the follow distance
            navMeshAgent.ResetPath();
        }
    }
}

