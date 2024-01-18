using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class RandomAnimalPlacer : MonoBehaviour
{
    public float spawnSize;
    public int spawnAmount;

    public GameObject[] animals;
    public static RandomAnimalPlacer Instance;

    public Transform[] spawnPoints;

    // [ContextMenu("Spawn Animals")]
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnAmount = spawnPoints.Length;
        RandomAnimalPlacer.Instance.SpawnAnimals();
    }

    public void SpawnAnimals() // nemažou se při vypnutí
    {
        var parent = new GameObject("-- ANIMALS --");

        for (int i = 0; i < spawnAmount; i++)
        {
            var value = Random.Range(0, animals.Length);

            Instantiate(animals[value], spawnPoints[0].position, Quaternion.identity, parent.transform);
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        // Generate a random direction within a unit sphere and scale it by the specified radius
        Vector3 randomDirection = Random.insideUnitSphere * radius;

        // Offset the random direction by the current transform position
        randomDirection += transform.position;

        // Declare a NavMeshHit variable to store information about the hit on the NavMesh
        NavMeshHit hit;

        // Declare a variable to store the final position, initialized to Vector3.zero
        Vector3 finalPosition = Vector3.zero;

        // Check if there is a valid NavMesh position within the specified radius of the random direction
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            // If a valid position is found, update the final position with the hit position
            finalPosition = hit.position;
        }

        // Return the final computed position on the NavMesh
        return finalPosition;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnSize);
    }
}