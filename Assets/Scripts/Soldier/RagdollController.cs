using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        rb.isKinematic = true; // Disable physics initially
    }

    public void ActivateRagdoll()
    {
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        rb.isKinematic = false; // Enable physics
        StartCoroutine(RotateCapsuleOnDeath());
    }

    private System.Collections.IEnumerator RotateCapsuleOnDeath()
    {
        float elapsedTime = 0f;
        float duration = 1f; // Time to rotate

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90f, 0f, 0f)); // Rotate 90 degrees on X

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation; // Ensure final rotation is applied
    }
}
