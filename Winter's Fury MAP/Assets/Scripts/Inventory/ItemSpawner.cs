using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory
{
    [System.Serializable]
    public class Item
    {
        public GameObject prefab;
        [Range(0f, 100f)] public float chance = 100f;
        [HideInInspector] public float weight;
    }
    
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private Item[] items;

        private void Start()
        {
            SpawnRandomItem();
        }

        private void SpawnRandomItem()
        {
            Item randomItem = items[ GetRandomItemIndex() ];

            var spawnedItem = Instantiate(randomItem.prefab, transform.position, randomItem.prefab.transform.rotation);

            if (Physics.Raycast(spawnedItem.transform.position, Vector3.down, out var hit, 10f))
            {
                spawnedItem.transform.position = hit.point;
            }
        }

        private int GetRandomItemIndex()
        {
            var accumulatedWeights = 0f;

            foreach (var item in items)
            {
                accumulatedWeights += item.chance;
                item.weight = accumulatedWeights;
            }

            var r = Random.value * accumulatedWeights;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].weight >= r)
                {
                    return i;
                }
            }
            
            return 0;
        }
    }
}