using Managers;
using UnityEngine;

namespace Heat
{
    public class InteriorBounds : MonoBehaviour
    {
        public float indoorTemperature;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = indoorTemperature;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = indoorTemperature;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TemperatureManager.indoorTemperature = 0;
            }
        }
    }
}
