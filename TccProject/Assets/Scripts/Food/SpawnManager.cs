using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    private float spawnRangeX = 16f;
    private float spawnPositionZ = 18f;
    private float startDelay = 2f;
    private float spawnInterval = 0.6f;

    void Start()
    {
        InvokeRepeating("SpawnAnimal", startDelay, spawnInterval);
    }

    void SpawnAnimal()
    {
        int animalIndex = Random.Range(0, animalPrefabs.Length);

        Vector3 randomPosition =
            new Vector3(Random.Range(-spawnRangeX, spawnRangeX), -74, spawnPositionZ);

        // Mantém a rotação original do prefab
        Instantiate(
            animalPrefabs[animalIndex],
            randomPosition,
            animalPrefabs[animalIndex].transform.rotation
        );
    }
}
