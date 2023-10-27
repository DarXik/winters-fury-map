using System;
using UnityEngine;

namespace UI
{
    public class Crosshair : MonoBehaviour
    {
        public Animator crosshairAnim;

        public static Crosshair Instance { get; set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            //FadeCrosshair();
        }

        public void FadeCrosshair()
        {
            crosshairAnim.SetTrigger("FadeCrosshair");
        }

        public void RevealCrosshair()
        {
            crosshairAnim.SetTrigger("RevealCrosshair");
        }
    }
}