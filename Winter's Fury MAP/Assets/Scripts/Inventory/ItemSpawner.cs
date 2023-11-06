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

            Instantiate(randomItem.prefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }

        private int GetRandomItemIndex()
        {
            var accumulatedWeights = 0f;

            foreach (var item in items)
            {
                accumulatedWeights += item.chance;
                item.weight = accumulatedWeights;
            }

            var r = Random.Range(0f, 1f) * accumulatedWeights;

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