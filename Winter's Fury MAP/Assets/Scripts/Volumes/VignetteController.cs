using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    public class VignetteController : MonoBehaviour
    {
        public Volume volume;
        private Vignette vignette;

        private bool strongVignette;
        
        public static VignetteController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            volume.profile.TryGet(out vignette);
        }

        public void ToggleVignette()
        {
            strongVignette = !strongVignette;

            if (strongVignette)
            {
                vignette.intensity.value = 0.443f;
            }
            else
            {
                vignette.intensity.value = 0.242f;
            }
        }
    }
}