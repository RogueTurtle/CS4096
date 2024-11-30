using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10;
    public float range = 50f;
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
        }
        else
        {
            Debug.Log("Shot missed!");
        }
    }
}
