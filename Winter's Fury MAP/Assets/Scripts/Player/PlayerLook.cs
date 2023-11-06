using System;
using UnityEngine;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        public float sensitivity;
        public Transform playerBody;

        private bool rotationBlocked = false;
        private float _xAxisClamp;

        public static PlayerLook Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            UnblockRotation();

            _xAxisClamp = 0;
        }

        public void BlockRotation()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            rotationBlocked = true;
        }

        public void UnblockRotation()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rotationBlocked = false;
        }

        private void LateUpdate()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            if (rotationBlocked) return;
            
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
