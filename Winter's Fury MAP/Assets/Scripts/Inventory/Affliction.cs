using UnityEngine;

namespace Inventory
{
    public enum AfflictionType
    {
        FoodPoisoning,
        Hypothermia
    }
    
    [System.Serializable]
    [CreateAssetMenu(fileName = "Affliction", menuName = "Scriptables/Affliction", order = 2)]
    public class Affliction : ScriptableObject
    {
        public string afflictionName;
        [TextArea] public string afflictionDescription;
        public Sprite afflictionIcon;
        public AfflictionType afflictionType;
        public bool hasSetDuration;
        public bool hasTreatment;
        
        [Header("Duration")] 
        [HideInInspector] public float treated;
        [HideInInspector] public float untreatedMin;
        [HideInInspector] public float untreatedMax;
        [HideInInspector] public float currentDuration;
        [HideInInspector] public float totalDuration;

        [Header("Treatment")] 
        [HideInInspector] public ItemData treatment;
        [HideInInspector] public int treatmentAmount;
        [HideInInspector] public bool wasTreated;
    }
}