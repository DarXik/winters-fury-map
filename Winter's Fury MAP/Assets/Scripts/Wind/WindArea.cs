using UnityEngine;

namespace Wind
{
    public class WindArea : MonoBehaviour
    {
        // set multiple levels of wind
        
        public float strength;
        public Vector3 direction;
    
        public static WindArea Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetWindDirection()
        {
            return direction;
        }
    }
}
