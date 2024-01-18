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
            // 3. VÝSTUP ONLY

            windIcon.SetActive(true);
        }

        public void HideWindIcon()
        {
            // 3. VÝSTUP ONLY

            windIcon.SetActive(false);
        }
    }
}