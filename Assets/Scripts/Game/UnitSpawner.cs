using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject redRangedPrefab;
    public GameObject redMeleePrefab;
    public GameObject blueRangedPrefab;
    public GameObject blueMeleePrefab;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    public int rangedCount = 10;
    public int meleeCount = 10;

    private void Start()
    {
        SpawnUnits(redSpawnPoints, "Team1", redRangedPrefab, redMeleePrefab, rangedCount, meleeCount);
        SpawnUnits(blueSpawnPoints, "Team2", blueRangedPrefab, blueMeleePrefab, rangedCount, meleeCount);
    }

    private void SpawnUnits(Transform[] spawnPoints, string teamTag, GameObject rangedPrefab, GameObject meleePrefab, int rangedUnits, int meleeUnits)
    {
        // Spawn ranged units
        for (int i = 0; i < rangedUnits; i++)
        {
            SpawnUnit(spawnPoints, teamTag, rangedPrefab);
        }

        // Spawn melee units
        for (int i = 0; i < meleeUnits; i++)
        {
            SpawnUnit(spawnPoints, teamTag, meleePrefab);
        }
    }

    private void SpawnUnit(Transform[] spawnPoints, string teamTag, GameObject unitPrefab)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Add some randomness to the spawn position
        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        GameObject unit = Instantiate(unitPrefab, spawnPosition, spawnPoint.rotation);

        // Set the team tag
        unit.tag = teamTag;
    }
}
