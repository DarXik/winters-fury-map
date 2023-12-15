using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        public GameObject backpack, crafting, condition;
        public Image backpackBtn, craftingBtn, conditionBtn;
        public Color32 activeColor, inactiveColor;

        private void Start()
        {
            DisplayBackpack();
        }

        public void DisplayBackpack()
        {
            backpack.SetActive(true);
            crafting.SetActive(false);
            condition.SetActive(false);

            backpackBtn.color = activeColor;
            craftingBtn.color = inactiveColor;
            conditionBtn.color = inactiveColor;
        }

        public void DisplayCrafting()
        {
            backpack.SetActive(false);
            crafting.SetActive(true);
            condition.SetActive(false);
            
            backpackBtn.color = inactiveColor;
            craftingBtn.color = activeColor;
            conditionBtn.color = inactiveColor;
        }

        public void DisplayCondition()
        {
            backpack.SetActive(false);
            crafting.SetActive(false);
            condition.SetActive(true);
            
            backpackBtn.color = inactiveColor;
            craftingBtn.color = inactiveColor;
            conditionBtn.color = activeColor;
        }
    }
}