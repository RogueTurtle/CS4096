using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10;
    public float range = 50f;
    SoldierAttributes soldierAttributes;
    public GameObject soldier;

    private void Start()
    {
        soldierAttributes = GetComponent<SoldierAttributes>();
        damage = soldierAttributes.GetDamage();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    public void Shoot() //Had to add public for this fsm could not access -Stephen
    {
        RaycastHit hit;
        if (Physics.Raycast(soldier.transform.position, soldier.transform.forward, out hit, range)) 
        {
            Debug.Log(hit.transform.name);

            Health target =  hit.transform.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
