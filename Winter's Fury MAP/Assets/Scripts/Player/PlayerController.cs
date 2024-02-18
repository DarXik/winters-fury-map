using System.Collections;
using Heat;
using Managers;
using UI;
using UnityEngine;
using Weather.Wind;

namespace Player
{
    public enum PlayerActivity
    {
        Sleeping,
        Standing,
        Walking,
        Running
    }

    public class PlayerController : MonoBehaviour
    {
        // References
        private CharacterController charController;
        [SerializeField] private Transform cameraTransform;
        private Vector3 initialCameraPos;

        [HideInInspector] public PlayerActivity currentActivity;

        [Header("Movement")] public float walkSpeed;
        public float runningSpeed;
        public float crouchSpeed;
        public float slopeSpeed;
        public float againstWindPenalty;
        public static bool isWindProtected;
        private float movementSpeed;
        private bool isRunning;
        private float horizontalInput, verticalInput;

        [Header("Stamina")] public float maxStamina;
        public float decreaseRate, increaseRate;
        public float timeToStartRegeneratingStamina;
        private float currentStamina;
        private bool isRegenerating, staminaDepleted;
        public float StaminaPercent => currentStamina / maxStamina;

        [Header("Crouching")] public float crouchHeight;
        public float crouchTransitionSpeed;
        private float standingHeight;
        private float currentHeight;
        private bool isCrouching;

        [Header("Slope Handling")] public float slopeForce;
        public float slopeForceRayLength;

        [Header("Utilities")] public Headbob headBob = new();

        public static PlayerController Instance { get; private set; }
        private bool headBobPref;

        private void Awake()
        {
            charController = GetComponent<CharacterController>();
            Instance = this;
        }

        private void Start()
        {
            standingHeight = currentHeight = charController.height;
            initialCameraPos = cameraTransform.localPosition;
            currentStamina = maxStamina;

            SetHeadBobPreference();

            headBob.Setup();
        }

        private void Update()
        {
            if (StaminaPercent <= 0f) StartCoroutine(StaminaDepletionHandler());
            if (isRegenerating) RegenerateStamina();
            
            if (!PlayerLook.rotationBlocked || VitalManager.playerDead)
            {
                MovingStateHandler();
                CheckWind();
                MovePlayer();
            }
        
            CrouchHandler();

            CheckForHeadBob();
            if (!isCrouching) headBob.ResetHeadBob();
        }

        private void CheckWind()
        {
            if (InteriorBounds.indoors)
            {
                WindUI.Instance.HideWindIcon();

                return;
            }
            
            var windDir = WindArea.Instance.GetWindDirection().normalized;

            float dotProduct = Vector3.Dot(-transform.forward, windDir);

            if (Physics.Raycast(transform.position, -windDir, 3f))
            {
                WindUI.Instance.DisplayWindIcon();
                isWindProtected = true;
            }
            else
            {
                WindUI.Instance.HideWindIcon();
                isWindProtected = false;
                
                if (dotProduct >= 0.775f && dotProduct <= 1f && WindArea.Instance.IsWindHigh())
                {
                    movementSpeed *= 1 - againstWindPenalty / 100f;
                }
            }
        }

        private void CheckForHeadBob()
        {
            if (!headBobPref) return;
            if (isRunning)
            {
                headBob.StartHeadBob();
            }
        }

        private void SetHeadBobPreference()
        {
            headBobPref  = PlayerPrefs.HasKey("headBobbingPreference") ? PlayerPrefs.GetInt("headBobbingPreference") == 1 : PlayerPrefs.GetInt("headBobbingPreference") == 0;
        }

        private void CrouchHandler()
        {
            var heightTarget = isCrouching ? crouchHeight : standingHeight;

            if (Mathf.Approximately(heightTarget, currentHeight)) return;

            var crouchDelta = Time.deltaTime * crouchTransitionSpeed;
            currentHeight = Mathf.Lerp(currentHeight, heightTarget, crouchDelta);

            var halfHeightDifference = new Vector3(0, (standingHeight - currentHeight) / 2f, 0);
            var newCameraPos = initialCameraPos - halfHeightDifference;

            cameraTransform.localPosition = newCameraPos;
            charController.height = currentHeight; 
        }

        private void MovePlayer()
        {
            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (verticalInput == 0 && horizontalInput == 0)
            {
                currentActivity = PlayerActivity.Standing;
                isRunning = false;
            }

            Vector3 forwardMovement = transform.forward * verticalInput;
            Vector3 rightMovement = transform.right * horizontalInput;

            charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f) * movementSpeed);

            // if player's moving on slope
            if ((verticalInput != 0 || horizontalInput != 0) && OnSlope())
            {
                charController.Move(Vector3.down * currentHeight / 2f * (slopeForce * Time.deltaTime));
            }
        }

        private IEnumerator StaminaDepletionHandler()
        {
            staminaDepleted = true;

            yield return new WaitUntil(() => StaminaPercent > 0.25f);

            staminaDepleted = false;
        }

        private void MovingStateHandler()
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !staminaDepleted &&
                Mathf.Approximately(charController.height, standingHeight))
            {
                if (horizontalInput == 0 && verticalInput == 0) return;
                
                if (VitalManager.Instance.FatiguePercent > 0)
                {
                    movementSpeed = runningSpeed;
                    isRunning = true;
                    currentActivity = PlayerActivity.Running;
                    
                    DecreaseStamina();
                    HUD.Instance.ShowStaminaIcon();
                }
                else
                {
                    movementSpeed = walkSpeed;
                    currentStamina = maxStamina;
                    HUD.Instance.ShowStaminaIcon(true);
                }
            }
            else
            {
                movementSpeed = isCrouching ? crouchSpeed : walkSpeed;

                isRunning = false;

                currentActivity = PlayerActivity.Walking;

                if (currentStamina < maxStamina) StartCoroutine(StartRegeneratingStamina());
                else HUD.Instance.FadeAwayStaminaIcon();
            }

            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
            }
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, currentHeight / 2f * slopeForceRayLength))
            {
                if (hit.normal != Vector3.up) return true;
            }

            return false;
        }

        private void DecreaseStamina()
        {
            if (isRegenerating) isRegenerating = false;
            StopAllCoroutines();

            currentStamina -= decreaseRate * Time.deltaTime;
        }

        private IEnumerator StartRegeneratingStamina()
        {
            yield return new WaitForSeconds(timeToStartRegeneratingStamina);

            isRegenerating = true;
            HUD.Instance.FadeAwayStaminaIcon();
        }

        private void RegenerateStamina()
        {
            if (currentStamina >= maxStamina)
            {
                isRegenerating = false;

                currentStamina = maxStamina;

                return;
            }

            currentStamina += increaseRate * Time.deltaTime;
        }

        public Vector3 GetPlayerPosition()
        {
            return transform.position;
        }

        public Transform GetPlayerTransform()
        {
            return transform;
        }
    }
}