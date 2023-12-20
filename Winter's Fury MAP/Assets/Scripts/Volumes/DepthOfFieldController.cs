using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Volumes
{
    public class DepthOfFieldController : MonoBehaviour
    {
        public Volume volume;
        private DepthOfField depthOfField;

        public float focusSpeed;
        public float maxFocusDistance;
    
        private Ray raycast;
        private RaycastHit hit;
        private float hitDistance;

        private bool blurred;
    
        public static DepthOfFieldController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            volume.profile.TryGet(out depthOfField);
        }

        private void Update()
        {
            if (blurred) return;
        
            raycast = new Ray(transform.position, transform.forward * maxFocusDistance);

            if (Physics.Raycast(raycast, out hit, 100f, ~LayerMask.GetMask("Interior")))
            {
                hitDistance = Vector3.Distance(transform.position, hit.point);
            }
            else
            {
                if (hitDistance < 100f)
                {
                    hitDistance++;
                }
            }
        
            SetFocus();
        }

        private void SetFocus()
        {
            depthOfField.focusDistance.value =
                Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.deltaTime * focusSpeed);
        }

        public void ToggleBlurScreen()
        {
            blurred = !blurred;

            if (blurred)
            {
                depthOfField.focusDistance.value = 0.1f;
            }
        }
    }
}
