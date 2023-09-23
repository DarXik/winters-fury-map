using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // References
    private CharacterController charController;

    [Header("Movement")] 
    public float walkSpeed;
    private float movementSpeed;
    private bool isRunning;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovingStateHandler();
        MovePlayer();
    }

    private void MovePlayer()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;

        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f) * movementSpeed);
    }

    private void MovingStateHandler()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            movementSpeed = walkSpeed;

            isRunning = false;
        }
    }
}
