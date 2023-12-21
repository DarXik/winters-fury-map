using System.Collections;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [Header("Stamina")] 
        [SerializeField] private GameObject staminaIcon;
        [SerializeField] private Image staminaFill, runningIcon;
        [SerializeField] private Gradient staminaGradient;
        [SerializeField] private Color unavailableColor;
        private Animator staminaAnim;
        
        public static HUD Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            staminaAnim = staminaIcon.GetComponent<Animator>();
        }

        private void Start()
        {
            staminaIcon.SetActive(false);
            staminaAnim.keepAnimatorStateOnDisable = false;
        }

        private void Update()
        {
            if (staminaIcon.activeInHierarchy && VitalManager.Instance.FatiguePercent > 0) HandleStaminaFill();
        }

        public void ShowStaminaIcon(bool emptyFatigue = false)
        {
            if (emptyFatigue)
            {
                staminaFill.color = unavailableColor;
                runningIcon.color = unavailableColor;
            }
            
            if(!staminaIcon.activeInHierarchy) staminaIcon.SetActive(true);
        }

        public void FadeAwayStaminaIcon()
        {
            staminaAnim.SetTrigger("FadeOut");
        }
        
        private void HandleStaminaFill()
        {
            float staminaPercent = PlayerController.Instance.StaminaPercent;
            
            staminaFill.fillAmount = staminaPercent;
            staminaFill.color = staminaGradient.Evaluate(staminaPercent);
            runningIcon.color = staminaGradient.Evaluate(staminaPercent);
        }
    }
}