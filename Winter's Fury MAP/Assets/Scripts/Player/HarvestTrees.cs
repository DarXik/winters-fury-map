using System;
using System.Collections.Generic;
using PolyPerfect;
using UnityEngine;

namespace Player
{
    public class HarvestTrees : MonoBehaviour
    {
        public float maxHitDistance;
        private List<TreeInstance> TreeInstances;
        private TerrainCollider terrainCollider;

        public static HarvestTrees Instance { get; set; }
        public bool CanRun;

        private void Awake()
        {
            Instance = this;
            terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        }

        private void Start()
        {
            CanRun = true;
            TreeInstances = new List<TreeInstance>(Terrain.activeTerrain.terrainData.treeInstances);
        }

        private void Update()
        {
            if (CanRun)
            {
                foreach (var animal in WanderScript.allAnimals)
                {
                    if (Vector3.Distance(animal.transform.position, transform.position) < WanderScript.Instance.awareness)
                    {
                        Debug.Log("called");
                        CanRun = false;
                        WanderScript.Instance.RunAwayFromAnimal(transform);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, maxHitDistance))
                {
                    if (hit.collider.name != Terrain.activeTerrain.name)
                    {
                        return;
                    }

                    float sampleHeight = Terrain.activeTerrain.SampleHeight(hit.point);

                    if (hit.point.y <= sampleHeight + 0.01f)
                    {
                        return;
                    }

                    TerrainData terrain = Terrain.activeTerrain.terrainData;
                    TreeInstance[] treeInstances = terrain.treeInstances;
                    TreePrototype currentTreePrototype = new TreePrototype();

                    float maxDistance = float.MaxValue;

                    Vector3 closestTreePosition = new Vector3();

                    int closestTreeIndex = 0;
                    for (int i = 0; i < treeInstances.Length; i++)
                    {
                        TreeInstance currentTree = treeInstances[i];

                        Vector3 currentTreeWorldPosition = Vector3.Scale(currentTree.position, terrain.size) +
                                                           Terrain.activeTerrain.transform.position;

                        float distance = Vector3.Distance(currentTreeWorldPosition, hit.point);

                        if (distance < maxDistance)
                        {
                            maxDistance = distance;
                            closestTreeIndex = i;
                            closestTreePosition = currentTreeWorldPosition;
                        }
                    }

                    // Remove the tree from the terrain tree list
                    currentTreePrototype =
                        terrain.treePrototypes[terrain.treeInstances[closestTreeIndex].prototypeIndex];
                    TreeInstances.RemoveAt(closestTreeIndex);
                    terrain.treeInstances = TreeInstances.ToArray();

                    // Refresh of the tree colliders
                    terrainCollider.enabled = false;
                    Debug.Log("");
                    terrainCollider.enabled = true;

                    // Put a falling tree in its place
                    Instantiate(currentTreePrototype.prefab, closestTreePosition, Quaternion.identity);
                }
            }
        }
    }
}