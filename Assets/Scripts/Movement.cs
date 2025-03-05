using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour
{
    CameraLockOn cameraLockOn;
    CharacterController controller;
    Animator animator;
    Combat combatScript;
    Teleport teleportScript;
    [SerializeField] Transform cam;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float jogSpeed = 6f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float rollSpeed = 10f;

    float playerSpeed = 1f;
    Vector3 playerVelocity;
    Vector2 moveInput;
    Vector2 tempMoveInput;
    Vector3 move;
    Vector3 rollDirection;
    bool isSprinting;
    bool isRolling;
    bool isJumping;


    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -9.81f;
    [SerializeField] bool groundedPlayer;
    [SerializeField] GameObject enemy;

    void Awake()
    {
        cameraLockOn = GetComponent<CameraLockOn>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        combatScript = GetComponent<Combat>();
        teleportScript = GetComponent<Teleport>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0.05f)
        {
            playerVelocity.y = 0f;
            isJumping = false;

            // get joystick position
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        animator.SetFloat("Speed", moveInput.magnitude);

        // send values to animator
        animator.SetFloat("HorizontalInput", moveInput.x);
        animator.SetFloat("VerticalInput", moveInput.y);

        move = new Vector3(moveInput.x, 0, moveInput.y);
        move = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * move;

        // Set the player movement speed based on the joycon position and sprint button
        if (moveInput.magnitude < 0.05f)
        {
            move = Vector3.zero;
        }

        else if (isSprinting)
        {
            playerSpeed = sprintSpeed;
            gameObject.transform.forward = move;
        }

        else if (moveInput.magnitude < 0.25f)
        {
            playerSpeed = walkSpeed;
        }

        else
        {
            playerSpeed = jogSpeed;
        }

        // If not locked on set forward direction to move direction
        if (!cameraLockOn.isLockedOn && moveInput.magnitude > 0.05f)
        {
            try
            {
                gameObject.transform.forward = move;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move the player depending on the current action
        if (isRolling)
        {
            if (cameraLockOn.isLockedOn)
            {
                transform.forward = rollDirection;
            }
            float adjustedRollSpeed = rollSpeed * (isSprinting ? 1.5f : 1f);
            controller.Move(transform.forward * Time.deltaTime * adjustedRollSpeed);
            moveInput = Vector2.zero;
        }
        // Stop movement if the player is attacking
        else if (combatScript.getIsPunching())
        {
            controller.Move(Vector3.zero);
        }
        // Apply movement velocity and vertical velocity in one go
        else
        {
            Vector3 finalVelocity = new Vector3(move.x * playerSpeed, playerVelocity.y, move.z * playerSpeed);
            controller.Move(finalVelocity * Time.deltaTime * 1 / Time.timeScale);
        }
    }

    public void setMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    void OnJump()
    {
        if (groundedPlayer)
        {
            tempMoveInput = moveInput;
            animator.SetBool("isJumping", true);
            moveInput = Vector2.zero;
        }
    }

    public void startJump()
    {
        isJumping = true;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public void stopJump()
    {
        animator.SetBool("isJumping", false);
    }

    void OnMove(InputValue value)
    {
        if (!combatScript.getIsPunching())
        {
            // get the raw value if the player is on the ground
            if (!isRolling && groundedPlayer)
            {
                moveInput = value.Get<Vector2>();
            }
            // if the player is jumping apply only half of the value to the movement
            else if (!groundedPlayer && !isRolling)
            {
                moveInput = tempMoveInput + value.Get<Vector2>() * 0.5f;
            }
            // if the player is rolling apply only half of the value to the movement 
            else if (isRolling)
            {
                moveInput += value.Get<Vector2>() * 0.5f;
            }
        }
    }

    void OnSprint(InputValue value)
    {
        if (value.isPressed && !isRolling && !isJumping && move.magnitude != 0 && !combatScript.getIsPunching())
        {
            isSprinting = true;
            animator.SetBool("isRunning", true);
        }
        else
        {
            turnOffSprint();
        }
    }

    public void turnOffSprint()
    {
        isSprinting = false;
        animator.SetBool("isRunning", false);
    }

    void OnRoll(InputValue value)
    {
        if (value.isPressed && !isJumping && !combatScript.getIsPunching() && !teleportScript.getIsTeleporting())
        {
            if (cameraLockOn.isLockedOn)
            {
                if (moveInput.magnitude >= 0.1)
                {
                    rollDirection = new Vector3(moveInput.x, 0, moveInput.y);
                    rollDirection = UnityEngine.Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * rollDirection;
                }
                else
                {
                    rollDirection = transform.forward;
                }
            }

            isRolling = true;
            animator.SetTrigger("Roll");
        }
    }

    public bool getIsRolling()
    {
        return isRolling;
    }

    public void stopRoll()
    {
        isRolling = false;
        moveInput = new Vector2(0f, 0.01f);
    }

    // Draw a sphere using the distance between the player and enemy
    void OnDrawGizmosSelected()
    {
        float distance = (enemy.transform.position - transform.position).magnitude;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
