
using UnityEngine;
using UnityEngine.AI;

public class FollowLeader : MonoBehaviour
{

    public Transform leader;
    public float followDistance = 3f;
    public int soldierNumber;
    
    private NavMeshAgent navMeshAgent;
    //public Vector3 offset;
    public Vector3[] formationOffsets = new Vector3[] {
        new Vector3(-2, 0, 2),   // Top-left
        new Vector3(2, 0, 2),    // Top-right
        new Vector3(-2, 0, -2),  // Bottom-left
        new Vector3(2, 0, -2)    // Bottom-right
    };
    public Vector3[] vFormationOffsets = new Vector3[] {
        new Vector3(-2, 0, -1),  // Left soldier, further behind and to the left
        new Vector3(-1, 0, -2),  // Second left soldier, behind and to the left
        new Vector3(1, 0, -2),   // Second right soldier, behind and to the right
        new Vector3(2, 0, -1)    // Right soldier, further behind and to the right
    };

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (leader != null)
        {
            // Get the follower's assigned offset based on its position in the formation
            int index = transform.GetSiblingIndex();
            index -= 1;
            Vector3 offset =vFormationOffsets[index];
            Debug.Log(gameObject.name +":"+ index);
            
            // Calculate the target position in the formation
            Vector3 targetPosition = leader.position + leader.TransformDirection(offset);

            // Move the follower to the target position using NavMesh
            navMeshAgent.SetDestination(targetPosition);

            navMeshAgent.avoidancePriority = Random.Range(0,100);//Assign unique priorities

        }
        else
            {
                // Stop moving when within the follow distance
                navMeshAgent.ResetPath();
            }
        }
    }

