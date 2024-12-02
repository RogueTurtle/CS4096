using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10; // Default damage value
    public float range = 50f; // Shooting range

    // Particle effect fields
    public GameObject muzzleFlashPrefab; // Assign this in the Inspector
    public GameObject impactEffectPrefab; // Assign this in the Inspector
    public Transform gunBarrel; // Assign this as the position where the muzzle flash will appear

    private SoldierAttributes soldierAttributes;

    private void Start()
    {
        // Get damage value from SoldierAttributes
        soldierAttributes = GetComponent<SoldierAttributes>();
        if (soldierAttributes != null)
        {
            damage = soldierAttributes.GetDamage();
        }
    }

    public void Shoot()
    {
        // Spawn the muzzle flash
        if (muzzleFlashPrefab != null && gunBarrel != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, gunBarrel.position, gunBarrel.rotation);
            Destroy(flash, 0.5f); // Destroy the flash after 0.5 seconds
        }

        RaycastHit hit;

        // Use the gunBarrel's position and forward direction for the raycast
        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, range))
        {
            Debug.Log($"Shot hit: {hit.transform.name}");

            // Check the tag of the hit object
            string shooterTag = gameObject.tag; // The shooting unit's tag (e.g., "Team1")
            string targetTag = shooterTag == "Team1" ? "Team2" : "Team1"; // Determine enemy tag

            if (hit.transform.CompareTag(targetTag))
            {
                // Apply damage if the target has a Health component
                Health target = hit.transform.GetComponent<Health>();
                if (target != null)
                {
                    Debug.Log($"Dealing {damage} damage to {hit.transform.name}");
                    target.TakeDamage(damage);

                    if (target.IsDead)
                    {
                        Debug.Log($"{hit.transform.name} has been killed.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Hit {hit.transform.name}, but it has no Health component.");
                }
            }
            else
            {
                Debug.Log($"Hit {hit.transform.name}, but it's not a valid target (tag: {hit.transform.tag}).");
            }

            // Spawn the impact effect at the hit point
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f); // Destroy the impact effect after 2 seconds
            }
        }
        else
        {
            Debug.Log("Shot missed!");
        }
    }
}
