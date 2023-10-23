using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum PlayerActivity
{
    Standing,
    Walking,
    Running
}

public enum PlayerAwakeness
{
    Awake,
    Asleep
}

public class PlayerController : MonoBehaviour
{
    // References
    private CharacterController charController;
    [SerializeField] private Transform cameraTransform;
    private Vector3 initialCameraPos;

    [HideInInspector] public PlayerActivity currentActivity;
    [HideInInspector] public PlayerAwakeness currentAwakeness;

    [Header("Movement")] public float walkSpeed;
    public float runningSpeed;
    public float crouchSpeed;
    private float movementSpeed;
    private bool isRunning;

    [Header("Stamina")] 
    public float maxStamina;
    public float decreaseRate, increaseRate;
    public float timeToStartRegeneratingStamina;
    private float currentStamina;
    private bool isRegenerating, staminaDepleted;
    private float StaminaPercent => currentStamina / maxStamina;

    [Header("Crouching")] public float crouchHeight;
    public float crouchTransitionSpeed;
    private float standingHeight;
    private float currentHeight;
    private bool isCrouching;

    [Header("Slope Handling")] public float slopeForce;
    public float slopeForceRayLength;

    [Header("Utilities")] public Headbob headBob = new();

    public static PlayerController Instance;

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

        headBob.Setup();

        currentAwakeness = PlayerAwakeness.Awake;
    }

    private void Update()
    {
        if(StaminaPercent <= 0f) StartCoroutine(StaminaDepletionHandler());
        
        MovingStateHandler();
        MovePlayer();
        CrouchHandler();
        HandleUI();

        CheckForHeadBob();
        headBob.ResetHeadBob();
        
        if(isRegenerating) RegenerateStamina();
    }

    private void CheckForHeadBob()
    {
        if (isRunning)
        {
            headBob.StartHeadBob();
        }
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
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (verticalInput == 0 && horizontalInput == 0) currentActivity = PlayerActivity.Standing;

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
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && currentStamina > 0f && !staminaDepleted)
        {
            movementSpeed = runningSpeed;

            isRunning = true;

            currentActivity = PlayerActivity.Running;

            DecreaseStamina();
        }
        else
        {
            movementSpeed = isCrouching ? crouchSpeed : walkSpeed;

            isRunning = false;

            currentActivity = PlayerActivity.Walking;

            if (currentStamina < maxStamina) StartCoroutine(StartRegeneratingStamina());
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }
    }

    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, currentHeight / 2f * slopeForceRayLength))
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
    }

    private void RegenerateStamina()
    {
        currentStamina += increaseRate * Time.deltaTime;

        if (currentStamina >= maxStamina)
        {
            isRegenerating = false;
        }
    }

    private void HandleUI()
    {
        
    }
}