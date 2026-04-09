using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;

    public float laneDistance = 5f;
    public float spawnZ = 20f;
    public float spawnInterval = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    void SpawnWave()
    {
        int safeLane = Random.Range(-1, 2);

        for (int lane = -1; lane <= 1; lane++)
        {
            if (lane == safeLane) continue;

            Vector3 spawnPos = new Vector3(lane * laneDistance, 0f, spawnZ);

            GameObject randomCar = carPrefabs[Random.Range(0, carPrefabs.Length)];
            Instantiate(randomCar, spawnPos, Quaternion.identity);
        }
    }
}