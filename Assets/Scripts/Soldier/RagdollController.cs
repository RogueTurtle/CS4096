using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Renderer objectRenderer; // Reference to the object's renderer
    public float colorTransitionDuration = 2f; // Time for the color transition

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        objectRenderer = GetComponent<Renderer>();

        rb.isKinematic = true; // Disable physics initially

        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component missing! Make sure your object has a material.");
        }
    }

    public void ActivateRagdoll()
    {
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        rb.isKinematic = false; // Enable physics

        // Start rotation and color transition coroutines
        StartCoroutine(RotateCapsuleOnDeath());
        if (objectRenderer != null)
        {
            StartCoroutine(ColorFlashToBlack());
        }
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

    private System.Collections.IEnumerator ColorFlashToBlack()
    {
        Color startColor = Color.white;
        Color endColor = Color.black;
        float elapsedTime = 0f;

        while (elapsedTime < colorTransitionDuration)
        {
            objectRenderer.material.color = Color.Lerp(startColor, endColor, elapsedTime / colorTransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set
        objectRenderer.material.color = endColor;
    }
}
