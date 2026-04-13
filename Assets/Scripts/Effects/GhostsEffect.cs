using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostsEffect : MonoBehaviour
{
    [SerializeField] private GameObject ghostImage;
    [SerializeField] private List<Transform> spawnPositions = new();
    private List<Vector3> spawners = new();

    [System.Serializable]
    public struct SpawnRate
    {
        public float minSpawnRate;
        public float maxSpawnRate;
    }

    [System.Serializable]
    public struct SpawnAmount
    {
        public int minSpawnAmount;
        public int maxSpawnAmount;
    }

    private int realSpawnAmount;
    private float realSpawnRate;

    [SerializeField] private SpawnAmount spawnAmount;
    [SerializeField] private SpawnRate spawnRate;

    private void Start()
    {
        foreach (Transform spanwer in spawnPositions)
        {
            spawners.Add(spanwer.position);
        }

        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        yield return null;

        while (true)
        {
            SpawnGhosts(realSpawnAmount);
            yield return new WaitForSeconds(realSpawnRate);
        }
    }

    private void SpawnGhosts(int ghostsToSpawn)
    {
        List<Vector3> tempSpawners = new List<Vector3>(spawners);

        int spawnCount = Mathf.Min(ghostsToSpawn, tempSpawners.Count);

        for (int i =0; i < spawnCount; i++)
        {
            int random = Random.Range(0, tempSpawners.Count);
            Ghost ghost = Instantiate(ghostImage, tempSpawners[random], Quaternion.identity).GetComponent<Ghost>();
            bool isLeft = tempSpawners[random].x > 0 ? true : false;
            ghost.Setup(isLeft);
            tempSpawners.RemoveAt(random);
        }
    }

    public void SetRates(float value)
    {
        float amount = Mathf.Lerp(spawnAmount.minSpawnAmount, spawnAmount.maxSpawnAmount, value);
        realSpawnAmount = Mathf.RoundToInt(amount);

        float rate = Mathf.Lerp(spawnRate.minSpawnRate, spawnRate.maxSpawnRate, value);
        realSpawnRate = rate;
    }
}
