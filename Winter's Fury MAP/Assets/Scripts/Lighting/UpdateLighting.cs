using System.Collections;
using UnityEngine;

namespace Lighting
{
    public class UpdateLighting : MonoBehaviour
    {
        public ReflectionProbe reflectionProbe;
        public float giUpdateTime;

        private void Awake()
        {
            StartCoroutine(UpdateEnvironmentLighting());
        }

        private IEnumerator UpdateEnvironmentLighting()
        {
            while (true)
            {
                DynamicGI.UpdateEnvironment();
                reflectionProbe.RenderProbe();
                
                yield return new WaitForSeconds(giUpdateTime);
            }
        } 
    }
}