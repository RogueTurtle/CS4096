using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject redTeamPrefab;
    public GameObject blueTeamPrefab;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    public int teamSize = 20;

    private void Start()
    {
        SpawnUnits(redSpawnPoints, "Team1");
        SpawnUnits(blueSpawnPoints, "Team2");
    }

    private void SpawnUnits(Transform[] spawnPoints, string teamTag)
    {
        for (int i = 0; i < teamSize; i++)
        {
            // Pick a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            GameObject unit = Instantiate(
                teamTag == "Team1" ? redTeamPrefab : blueTeamPrefab,
                spawnPosition,
                spawnPoint.rotation
            );

            // Set the team tag
            unit.tag = teamTag;
        }
    }
}
