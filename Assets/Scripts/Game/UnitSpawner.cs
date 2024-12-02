using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject redRangedPrefab;
    public GameObject redMeleePrefab;
    public GameObject blueRangedPrefab;
    public GameObject blueMeleePrefab;
    public GameObject redGeneralPrefab; 
    public GameObject blueGeneralPrefab; 

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    public Transform retreatPointTeam1; // Retreat point for Team1
    public Transform retreatPointTeam2; // Retreat point for Team2

    public int rangedCount = 10;
    public int meleeCount = 10;
    public int generalCount = 1; // Number of generals per team 

    private void Start()
    {
        // Spawn Red Team Units
        SpawnUnits(redSpawnPoints, "Team1", redRangedPrefab, redMeleePrefab, rangedCount, meleeCount, retreatPointTeam1);
        SpawnGenerals(redSpawnPoints, "Team1", redGeneralPrefab, generalCount, retreatPointTeam1);

        // Spawn Blue Team Units
        SpawnUnits(blueSpawnPoints, "Team2", blueRangedPrefab, blueMeleePrefab, rangedCount, meleeCount, retreatPointTeam2);
        SpawnGenerals(blueSpawnPoints, "Team2", blueGeneralPrefab, generalCount, retreatPointTeam2);
    }

    private void SpawnUnits(Transform[] spawnPoints, string teamTag, GameObject rangedPrefab, GameObject meleePrefab, int rangedUnits, int meleeUnits, Transform retreatPoint)
    {
        // Spawn ranged units
        for (int i = 0; i < rangedUnits; i++)
        {
            SpawnUnit(spawnPoints, teamTag, rangedPrefab, retreatPoint);
        }

        // Spawn melee units
        for (int i = 0; i < meleeUnits; i++)
        {
            SpawnUnit(spawnPoints, teamTag, meleePrefab, retreatPoint);
        }
    }

    private void SpawnGenerals(Transform[] spawnPoints, string teamTag, GameObject generalPrefab, int generalUnits, Transform retreatPoint)
    {
        // Spawn generals
        for (int i = 0; i < generalUnits; i++)
        {
            SpawnUnit(spawnPoints, teamTag, generalPrefab, retreatPoint);
        }
    }

    private void SpawnUnit(Transform[] spawnPoints, string teamTag, GameObject unitPrefab, Transform retreatPoint)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Add some randomness to the spawn position
        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        GameObject unit = Instantiate(unitPrefab, spawnPosition, spawnPoint.rotation);

        // Set the team tag
        unit.tag = teamTag;

        // Assign the retreat point dynamically
        FSM ai = unit.GetComponent<FSM>();
        if (ai != null)
        {
            ai.retreatPoint = retreatPoint;
        }
    }
}
