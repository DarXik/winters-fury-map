using System.Collections;
using Inventory;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Volumes;
using Weather.Wind;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Core References")] public GameObject backpack;
        public GameObject crafting;
        public GameObject condition;
        public GameObject afflictionItem;
        public GameObject noAfflictionItem;
        public Transform afflictionContainer;
        public Image backpackBtn, craftingBtn, conditionBtn;
        public Color32 activeColor, inactiveColor;

        [Header("Status")] public TextMeshProUGUI conditionText;
        public Slider warmthBar;
        public Slider fatigueBar;
        public Slider thirstBar;
        public Slider hungerBar;
        public TextMeshProUGUI feelsLikeText, airTempText, windChillText;
        public Color32 lowTempColor, normalTempColor;

        [Header("Treatment")] public GameObject treatmentObj;
        public TextMeshProUGUI afflictionDesc;
        public Image treatmentIcon;
        public TextMeshProUGUI treatmentAmount;
        public TextMeshProUGUI recoveryTimeText;

        [Header("Treatment Chooser")] 
        public GameObject treatmentChooser;
        public GameObject listOfTreatments, timeText, recoveryTimes;
        public GameObject wasTreatedObj;
        public Image affIconImage;
        public TextMeshProUGUI affNameText;
        public Button treatAfflictionBtn;

        [Header("Danger Icons")] public GameObject stomachDanger;

        [Header("Affliction Alert")] public GameObject afflictionAlert;
        private Animator afflictionAlertAnim;
        public TextMeshProUGUI nameToDisplay;
        public Image iconToDisplay;

        private bool noAffliction;
        private Affliction afflictionToTreat;
        private int index;

        private GameObject selectedAffItem;

        public static InventoryUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            afflictionAlertAnim = afflictionAlert.GetComponent<Animator>();
        }

        private void Start()
        {
            DisplayBackpack();

            stomachDanger.SetActive(false);
            treatmentObj.SetActive(false);
            treatmentChooser.SetActive(false);
            afflictionAlert.SetActive(false);

            noAffliction = true;
            Instantiate(noAfflictionItem, afflictionContainer);
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

        public IEnumerator DisplayAfflictionAlert(string name, Sprite icon)
        {
            afflictionAlert.SetActive(true);
            nameToDisplay.text = name;
            iconToDisplay.sprite = icon;

            yield return new WaitForSeconds(2f);

            afflictionAlertAnim.SetTrigger("HideAlert");
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

            treatmentObj.SetActive(false);

            if (afflictions.Count > 0)
            {
                DeleteAffContainerContent();
                noAffliction = false;

                foreach (var affliction in afflictions)
                {
                    var affItem = Instantiate(afflictionItem, afflictionContainer);
                    affItem.transform.Find("AfflictionName").GetComponent<TextMeshProUGUI>().text =
                        affliction.afflictionName;
                    affItem.transform.Find("AfflictionIconBG/AfflictionIcon").GetComponent<Image>().sprite =
                        affliction.afflictionIcon;
                    affItem.transform.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (selectedAffItem != null)
                        {
                            selectedAffItem.GetComponent<Image>().enabled = false;
                        }
                        
                        affItem.GetComponent<Image>().enabled = true;

                        selectedAffItem = affItem;
                        
                        DisplayTreatment(affliction);
                    });

                    if (affliction.afflictionType == AfflictionType.FoodPoisoning)
                    {
                        stomachDanger.SetActive(true);
                    }
                }
            }
            else if (afflictions.Count == 0 && !noAffliction)
            {
                DeleteAffContainerContent();
                noAffliction = true;

                Instantiate(noAfflictionItem, afflictionContainer);
            }
        }

        private void DisplayTreatment(Affliction affliction)
        {
            treatmentObj.SetActive(true);

            afflictionDesc.text = affliction.afflictionDescription;

            if (affliction.hasTreatment)
            {
                listOfTreatments.SetActive(true);
                timeText.SetActive(true);
                recoveryTimes.SetActive(true);
                
                treatmentIcon.sprite = affliction.treatment.itemIcon;
                treatmentIcon.preserveAspect = true;
                treatmentAmount.text = affliction.treatmentAmount.ToString();
                wasTreatedObj.SetActive(affliction.wasTreated);

                recoveryTimeText.text =
                    $"{Mathf.RoundToInt(affliction.currentDuration)} HOURS / {Mathf.RoundToInt(affliction.totalDuration)} HOURS";
            }
            else
            {
                listOfTreatments.SetActive(false);
                timeText.SetActive(false);
                recoveryTimes.SetActive(false);
            }
        }

        public void ShowTreatmentChooser(ItemData treatmentData, int itemCount)
        {
            index = 0;

            var afflictions = VitalManager.Instance.GetCurrentAfflictions();

            // display something else when itemCount is not suitable
            if (afflictions.Count == 0) return;

            treatmentChooser.SetActive(true);
            DepthOfFieldController.Instance.ToggleBlurScreen();
            InventoryManager.Instance.ToggleInventory(false);

            afflictionToTreat = afflictions[index];

            affIconImage.sprite = afflictionToTreat.afflictionIcon;
            affNameText.text = afflictionToTreat.afflictionName;

            treatAfflictionBtn.onClick.AddListener(() =>
            {
                VitalManager.Instance.TreatAffliction(treatmentData, afflictionToTreat.treatmentAmount,
                    afflictionToTreat);
            });
        }

        public void HideTreatmentChooser()
        {
            treatmentChooser.SetActive(false);
            DepthOfFieldController.Instance.ToggleBlurScreen();
            InventoryManager.Instance.ToggleInventory(false);
        }

        public void NextAffliction()
        {
            var afflictionCount = VitalManager.Instance.GetCurrentAfflictions().Count;

            if (index < afflictionCount - 1)
            {
                index++;
            }

            UpdateTreatmentChooser();
        }

        public void PreviousAffliction()
        {
            if (index > 0)
            {
                index--;
            }

            UpdateTreatmentChooser();
        }

        private void UpdateTreatmentChooser()
        {
            var afflictions = VitalManager.Instance.GetCurrentAfflictions();

            afflictionToTreat = afflictions[index];

            affIconImage.sprite = afflictionToTreat.afflictionIcon;
            affNameText.text = afflictionToTreat.afflictionName;
        }

        private void DeleteAffContainerContent()
        {
            foreach (Transform aff in afflictionContainer)
            {
                Destroy(aff.gameObject);
            }

            stomachDanger.SetActive(false);
        }
    }
}