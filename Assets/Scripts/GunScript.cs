using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10;
    public float range = 50f;

    public GameObject soldier;
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(soldier.transform.position, soldier.transform.forward, out hit, range)) 
        {
            Debug.Log(hit.transform.name);

            SoldierAttributes target =  hit.transform.GetComponent<SoldierAttributes>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
