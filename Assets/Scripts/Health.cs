using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    private SoldierAttributes soldierAttributes;
    private void Start()
    {
        soldierAttributes = GetComponent<SoldierAttributes>();

        health = soldierAttributes.GetHealth();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Dead");
        Destroy(gameObject);
    }
}
