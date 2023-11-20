using System;
using System.Collections;
using UnityEngine;

namespace Lighting
{
    public class UpdateLighting : MonoBehaviour
    {
        [SerializeField] private ReflectionProbe reflectionProbe;
        [SerializeField] private float giUpdateTime;
        
        public static UpdateLighting Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DynamicGI.UpdateEnvironment();
            reflectionProbe.RenderProbe();
            
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

        public void ForceUpdateEnvironmentLighting()
        {
            DynamicGI.UpdateEnvironment();
            reflectionProbe.RenderProbe();
        }
    }
}
