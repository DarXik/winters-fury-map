using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public enum InteractableType
    {
        Door,
        Bed,
        Searchable
    }

    [Serializable]
    public class Item
    {
        public ItemData itemData;
        public float chance = 100f;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Interactable", menuName = "Scriptables/Interactable", order = 2)]
    public class InteractionData : ScriptableObject
    {
        public string interactionText;
        public InteractableType interactableType;
        [HideInInspector] public bool searched;

        [Header("Bed Properties")] [HideInInspector]
        public float warmthBonus;

        [Header("Searchable Properties")] [HideInInspector]
        public List<Item> items;
    }
}