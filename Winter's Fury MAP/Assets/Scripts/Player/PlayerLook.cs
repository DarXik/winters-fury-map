using UnityEngine;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        public float sensitivity;
        public Transform playerBody;

        private float _xAxisClamp;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _xAxisClamp = 0;
        }

        private void LateUpdate()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

            _xAxisClamp += mouseY;

            if (_xAxisClamp > 90f)
            {
                _xAxisClamp = 90f;
                mouseY = 0;
                
                ClampXAxisRotationValue(270f);
            }
            else if (_xAxisClamp < -90f)
            {
                _xAxisClamp = -90f;
                mouseY = 0;
                
                ClampXAxisRotationValue(90f);
            }
            
            transform.Rotate(Vector3.left * mouseY);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        
        private void ClampXAxisRotationValue(float value)
        {
            Vector3 eulerRotation = transform.eulerAngles;

            eulerRotation.x = value;
            transform.eulerAngles = eulerRotation;
        }
    }
}
