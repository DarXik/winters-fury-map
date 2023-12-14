using UnityEngine;

namespace Weather.Wind
{
    public class WindUI : MonoBehaviour
    {
        public GameObject windIcon;
        
        public static WindUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            windIcon.SetActive(false);
        }

        public void DisplayWindIcon()
        {
            windIcon.SetActive(true);
        }
        public void HideWindIcon()
        {
            windIcon.SetActive(false);
        }
        
    }
}