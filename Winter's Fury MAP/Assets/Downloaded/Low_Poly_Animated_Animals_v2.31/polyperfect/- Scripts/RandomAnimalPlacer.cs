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
    public Transform[] spawnPoints;


    private void Start()
    {
        SpawnAnimals();
    }

    public void SpawnAnimals()
    {
        GameObject parent = GameObject.Find("-- ANIMALS --");

        if (parent == null)
        {
            parent = new GameObject("-- ANIMALS --");
        }

        Transform parentTransform = parent.transform;
        int numOfChildren = parentTransform.childCount;

        if (numOfChildren < spawnAmount)
        {
            for (int i = 0; i < spawnAmount - numOfChildren; i++)
            {
                var value = Random.Range(0, animals.Length);
                Instantiate(animals[value], RandomNavmeshLocation(750), Quaternion.identity, parent.transform);
            }
        }
    }

    public GameObject spawnPoint;

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;

        randomDirection += spawnPoint.transform.position;

        NavMeshHit hit;

        Vector3 finalPosition = Vector3.zero;

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