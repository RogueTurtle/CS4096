using UnityEngine;

public class Morale : MonoBehaviour
{
    private float morale;
    private float moraleBoost;
    private float moralePenalty;
    SoldierAttributes soldierAttributes;

    private void Start()
    {
        soldierAttributes = GetComponent<SoldierAttributes>();
        morale = soldierAttributes.GetMorale();
    }

    public void UpdateMorale(bool isBoost)
    {
        morale += isBoost ? moraleBoost : -moralePenalty;
        morale = Mathf.Clamp(morale, 0, 100);

        if(morale <= 10)
        {
            Debug.Log("Routing");
            //TODO expand morale logic and integrate with AI
        }
    }
}
