using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10;
    public float range = 50f;

    // Particle effect fields
    public GameObject muzzleFlashPrefab; // Assign this in the Inspector
    public GameObject impactEffectPrefab; // Assign this in the Inspector
    public Transform gunBarrel; // Assign this as the position where the muzzle flash will appear

    SoldierAttributes soldierAttributes;

    private void Start()
    {
        // Get damage from SoldierAttributes
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

        // Use the GameObject's position and forward direction
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            Debug.Log($"Shot hit: {hit.transform.name}");

            // Apply damage if the target has a Health component
            Health target = hit.transform.GetComponent<Health>();
            if (target != null)
            {
                Debug.Log($"Dealing {damage} damage to {hit.transform.name}");
                target.TakeDamage(damage);

                if (target.health <= 0)
                {
                    Debug.Log($"{hit.transform.name} has been killed.");
                }
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
