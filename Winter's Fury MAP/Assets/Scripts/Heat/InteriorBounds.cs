using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Heat
{
    public class InteriorBounds : MonoBehaviour
    {
        public float indoorTemperatureBonus;

        public static bool indoors;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.IndoorTemperature = indoorTemperatureBonus;
                indoors = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.IndoorTemperature = indoorTemperatureBonus;
                indoors = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.IndoorTemperature = 0;
                indoors = false;
            }
        }
    }
}
