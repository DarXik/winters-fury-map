using UnityEngine;

namespace Inventory
{
    public enum AfflictionType
    {
        FoodPoisoning
    }
    
    [System.Serializable]
    [CreateAssetMenu(fileName = "Affliction", menuName = "Scriptables/Affliction", order = 2)]
    public class Affliction : ScriptableObject
    {
        public string afflictionName;
        [TextArea] public string afflictionDescription;
        public Sprite afflictionIcon;
        public AfflictionType afflictionType;
        [Header("Recovery Times")]
        [Range(1f, 24f)] public float recoveryTimeTreated;
        [Range(1f, 24f)] public float recoveryTimeUntreatedMin;
        [Range(1f, 24f)] public float recoveryTimeUntreatedMax;
        private float recoveryTimeUntreated;

        public float RecoveryTimeUntreated
        {
            get
            {
                if (recoveryTimeUntreated == 0f)
                {
                    recoveryTimeUntreated = Random.Range(recoveryTimeUntreatedMin, recoveryTimeUntreatedMax);
                }
                
                return recoveryTimeUntreated;
            }
        }
    }
}