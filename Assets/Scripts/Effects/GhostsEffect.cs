using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostsEffect : MonoBehaviour
{
    [SerializeField] private GameObject ghostImage;
    [SerializeField] private List<Transform> spawnPositions = new();
    private List<Vector3> spawners = new();
    [SerializeField] private float spawnRate = 3;

    [System.Serializable]
    public struct SpawnAmount
    {
        public int minSpawnAmount;
        public int maxSpawnAmount;
    }

    [SerializeField] private SpawnAmount spawnAmount;

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
        while (true)
        {
            int randomGhosts = Random.Range(spawnAmount.minSpawnAmount, spawnAmount.maxSpawnAmount);

            SpawnGhosts(randomGhosts);
            yield return new WaitForSeconds(spawnRate);
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
}
