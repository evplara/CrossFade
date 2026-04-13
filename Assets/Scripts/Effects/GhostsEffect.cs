using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostsEffect : MonoBehaviour
{
    [SerializeField] private GameObject ghostImage;
    [SerializeField] private List<Transform> spawnPositions = new();
    private List<Vector3> spawners = new();

    [System.Serializable]
    public struct SpawnRate
    {
        public int minSpawnRate;
        public int maxSpawnRate;
    }

    [System.Serializable]
    public struct SpawnAmount
    {
        public int minSpawnAmount;
        public int maxSpawnAmount;
    }

    private int realSpawnAmount;
    private int realSpawnRate;

    [SerializeField] private SpawnAmount spawnAmount;
    [SerializeField] private SpawnAmount spawnRate;

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

        for (int i =0; i < ghostsToSpawn; i++)
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

        float rate = Mathf.Lerp(spawnRate.minSpawnAmount, spawnRate.maxSpawnAmount, value);
        realSpawnRate = Mathf.RoundToInt(rate);
    }
}
