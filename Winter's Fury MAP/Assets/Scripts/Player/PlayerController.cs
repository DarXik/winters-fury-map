using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // References
    private CharacterController charController;
    [SerializeField] private Transform cameraTransform;
    private Vector3 initialCameraPos;

    [Header("Movement")] 
    public float walkSpeed;
    public float runningSpeed;
    public float crouchSpeed;
    private float movementSpeed;
    private bool isRunning;

    [Header("Crouching")] 
    public float crouchHeight;
    public float crouchTransitionSpeed;
    private float standingHeight;
    private float currentHeight;
    private bool isCrouching;

    [Header("Slope Handling")] 
    public float slopeForce;
    public float slopeForceRayLength;
    

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        standingHeight = currentHeight = charController.height;
        initialCameraPos = cameraTransform.localPosition;
    }

    private void Update()
    {
        MovingStateHandler();
        MovePlayer();
        CrouchHandler();
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

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;

        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f) * movementSpeed);
        
        // if player's moving on slope
        if ((verticalInput != 0 || horizontalInput != 0) && OnSlope())
        {
            charController.Move(Vector3.down * currentHeight / 2f * (slopeForce * Time.deltaTime));
        }
    }

    private void MovingStateHandler()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = runningSpeed;
            
            isRunning = true;
        }
        else
        {
            movementSpeed = walkSpeed;

            isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            movementSpeed = crouchSpeed;
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
}
