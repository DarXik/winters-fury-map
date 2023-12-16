using Inventory;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weather.Wind;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Core References")] 
        public GameObject backpack;
        public GameObject crafting;
        public GameObject condition;
        public GameObject afflictionItem;
        public Transform afflictionContainer;
        public Image backpackBtn, craftingBtn, conditionBtn;
        public Color32 activeColor, inactiveColor;

        [Header("Condition")] 
        public TextMeshProUGUI conditionText;
        public Slider warmthBar;
        public Slider fatigueBar;
        public Slider thirstBar;
        public Slider hungerBar;
        public TextMeshProUGUI feelsLikeText, airTempText, windChillText;
        public Color32 lowTempColor, normalTempColor;

        [Header("Danger Icons")] 
        public GameObject stomachDanger;
        
        public static InventoryUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DisplayBackpack();
            
            stomachDanger.SetActive(false);
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
            
            UpdateConditionUI();
        }

        public void UpdateConditionUI()
        {
            // Assign sliders
            warmthBar.value = VitalManager.Instance.WarmthPercent;
            fatigueBar.value = VitalManager.Instance.FatiguePercent;
            thirstBar.value = VitalManager.Instance.ThirstPercent;
            hungerBar.value = VitalManager.Instance.HungerPercent;
            
            // Display texts
            conditionText.text = $"{Mathf.RoundToInt(VitalManager.Instance.CurrentHealth)}%";
            feelsLikeText.text = $"{Mathf.RoundToInt(TemperatureManager.Instance.FeelsLike)}°C";
            airTempText.text = $"{Mathf.RoundToInt(TemperatureManager.Instance.AmbientTemperature)}°C";

            if (WindArea.Instance.GetWindChill() > 0)
            {
                windChillText.text = $"-{Mathf.RoundToInt(WindArea.Instance.GetWindChill())}°C";
                windChillText.color = lowTempColor;
            }
            else
            {
                windChillText.text = $"{Mathf.RoundToInt(WindArea.Instance.GetWindChill())}°C";
                windChillText.color = normalTempColor;
            }

            airTempText.color = TemperatureManager.Instance.AmbientTemperature < 1 ? lowTempColor : normalTempColor;
            
            // Check for afflictions
            var afflictions = VitalManager.Instance.GetCurrentAfflictions();

            if (afflictions.Count > 0)
            {
                DeleteAffContainerContent();
                
                foreach (var affliction in afflictions)
                {
                    var affItem = Instantiate(afflictionItem, afflictionContainer);
                    affItem.transform.Find("AfflictionName").GetComponent<TextMeshProUGUI>().text =
                        affliction.afflictionName;
                    affItem.transform.Find("AfflictionIconBG/AfflictionIcon").GetComponent<Image>().sprite =
                        affliction.afflictionIcon;

                    if (affliction.afflictionType == AfflictionType.FoodPoisoning)
                    {
                        stomachDanger.SetActive(true);
                    }
                }
            }
        }

        private void DeleteAffContainerContent()
        {
            foreach (Transform aff in afflictionContainer)
            {
                Destroy(aff.gameObject);
            }
        }
    }
}