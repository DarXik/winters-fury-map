using Managers;
using UnityEngine;

namespace Heat
{
    public class InteriorBounds : MonoBehaviour
    {
        public float indoorTemperature;

        public static bool indoors;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = indoorTemperature;
                indoors = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = indoorTemperature;
                indoors = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = 0;
                indoors = false;
            }
        }
    }
}
