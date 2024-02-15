using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        public float sensitivity;
        public Transform playerBody;

        public static bool rotationBlocked;
        private float _xAxisClamp;

        public static PlayerLook Instance { get; private set; }

        public void SetSensitivity()
        {
            sensitivity = PlayerPrefs.GetFloat("sensitivityPreference");
        }
        
        private void Awake()
        {

            Instance = this;
        }
        private void Update()
        {
            SetSensitivity();
        }
        private void Start()
        {

            UnblockRotation();

            _xAxisClamp = 0;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        public void BlockRotation(bool cursorVisible = true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = cursorVisible;
            rotationBlocked = true;
        }

        public void UnblockRotation(bool cursorVisible = false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = cursorVisible;
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

        public IEnumerator LookAt(Transform target)
        {
            var speed = 1f;
            var timeElapsed = 0f;

            var targetRotation = Quaternion.LookRotation(target.position - transform.position);
            
            while (timeElapsed < speed)
            {
                transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, Quaternion.Euler(targetRotation.eulerAngles.x, 0, 0), timeElapsed / speed);
                playerBody.rotation = Quaternion.SlerpUnclamped(playerBody.rotation, Quaternion.Euler(0, targetRotation.eulerAngles.y, 0), timeElapsed / speed);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            
        }
    }
}
