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

        private bool rotationBlocked = false;
        private float _xAxisClamp;
        private Quaternion startingRotation;

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

            if (Input.GetKeyDown(KeyCode.P))
            {
                transform.rotation = startingRotation;
            }
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
