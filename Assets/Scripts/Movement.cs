using System.Collections;
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
    bool jumpStarted = false;


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
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0.05f)
        {
            moveInput = Vector2.zero;
            playerVelocity.y = 0f;
            isJumping = false;
            if(!jumpStarted)
            {
                animator.SetBool("isJumping", false);
            }  

            // get joystick position
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        // if(!isJumping)
        // {
            animator.SetFloat("Speed", moveInput.magnitude); 

            // send values to animator
            animator.SetFloat("HorizontalInput", moveInput.x);
            animator.SetFloat("VerticalInput", moveInput.y);

            move = new Vector3(moveInput.x, 0, moveInput.y);
            move = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * move;
        // }
        
        if(moveInput.magnitude < 0.05f)
        {
            // animator.SetBool("isStatic", true);
            move = Vector3.zero;
        }

        else if(isSprinting)
        {
            playerSpeed = sprintSpeed;
            // gameObject.transform.forward = move;
        }

        else if(moveInput.magnitude < 0.25f)
        {
            // animator.SetBool("isRunning", false);

            playerSpeed = walkSpeed;
            // gameObject.transform.forward = move;
        }

        else
        {
            // animator.SetBool("isRunning", true);
            // animator.SetBool("isWalking", false);

            playerSpeed = jogSpeed;
            // gameObject.transform.forward = move;
        }

        // if not locked on set forward direction to move direction
        if(!cameraLockOn.isLockedOn && moveInput.magnitude > 0.05f)
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

        playerVelocity.y += gravityValue * Time.deltaTime;
    
        if(isRolling)
        {
            if(cameraLockOn.isLockedOn)
            {
                transform.forward = rollDirection;
            }
            controller.Move(transform.forward * Time.deltaTime * rollSpeed);            
            moveInput = Vector2.zero;
        }
        // stop movement if the player is attacking
        else if (combatScript.getIsPunching()) {
            controller.Move(Vector3.zero);
        }
        // Apply movement velocity and vertical velocity in one go
        else
        {
            Vector3 finalVelocity = new Vector3(move.x * playerSpeed, playerVelocity.y, move.z * playerSpeed);
            controller.Move(finalVelocity * Time.deltaTime * 1/Time.timeScale);
        }
    }

    public void setMoveInput(Vector2 input) {
        moveInput = input;
    }

     public void turnOffSprint()
    {
        isSprinting = false;
        animator.SetBool("isRunning", false);
    }

    void OnJump(InputValue value)
    {
        if(groundedPlayer)
        {
            tempMoveInput = moveInput;
            jumpStarted = true;
            StartCoroutine(AdjustJumpTiming());
            turnOffSprint();
        }
    }

    void OnMove(InputValue value)
    {
        if(!combatScript.getIsPunching()) {
            // get the raw value if the player is on the ground
            if(!isRolling && groundedPlayer )
            {
                moveInput = value.Get<Vector2>();
            }
            // if the player is jumping add half of the value to the movement before the jump started
            else if (!groundedPlayer && !isRolling)
            {
                moveInput = tempMoveInput + value.Get<Vector2>() *  0.5f;
            }  
            // if the player is rolling add half of the value to the movement before the roll started
            else if (isRolling) {
                moveInput += value.Get<Vector2>() *  0.5f;
            } 
        }
    }

    void OnSprint(InputValue value)
    {
        if(!isRolling && !isJumping && move.magnitude != 0 && !combatScript.getIsPunching())
        {
            if (value.isPressed)
            {
                isSprinting = true;
                animator.SetBool("isRunning", true);
            }
            else
            {
                turnOffSprint();
            }
        }
        else
        {
            turnOffSprint();
        }
        
    }

    void OnRoll(InputValue value)
    {
        if(value.isPressed && !isJumping && !combatScript.getIsPunching())
        {
            // turnOffSprint();

            if(cameraLockOn.isLockedOn)
            {
                if(moveInput.magnitude >= 0.1)
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

            StartCoroutine(ResetRollState(1f));
        }
    }

    IEnumerator ResetRollState(float len)
    {

        yield return new WaitForSeconds(len);
        isRolling = false;
        // animator.SetBool("isPunching", false);
        moveInput = new Vector2 (0f, 0.01f);
        yield return new WaitForEndOfFrame();
    }

    // TODO: FIX JUMP TIMING IN SLOW MO MODE
    IEnumerator AdjustJumpTiming()
    {
        animator.SetBool("isJumping", true);
        moveInput = Vector2.zero;
        yield return new WaitForSeconds(0.6f * Time.timeScale);
        isJumping = true;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        yield return new WaitForSeconds(0.8f);
        jumpStarted = false;
    }

    void OnDrawGizmosSelected() {
        float distance = (enemy.transform.position - transform.position).magnitude;
        Gizmos.DrawWireSphere(transform.position, distance);
    }

}
