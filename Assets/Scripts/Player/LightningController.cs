using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightningMovement : MonoBehaviour
{
    public float InputX;
    public float InputZ;
    float Speed;
    public float moveSpeed = 10f;
    Vector3 finalMoveDirection;
    public float allowPlayerRotation = 0.1f;
    public float desiredRotationSpeed = 0.1f;

    Animator animator;
    Camera cam;
    CharacterController controller;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    Vector3 dashMoveDirection = Vector3.zero;
    bool isDashing = false;

    [Header("Ascend/Descend")]

    [SerializeField] InputActionReference ascendActionReference;
    [SerializeField] InputActionReference descendActionReference;
    InputAction ascendAction;
    InputAction descendAction;
    public float ascendSpeed = 5f;
    Vector3 ascendMoveDirection = Vector3.zero;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();

        // Initialise ascend/descend actions
        ascendAction = ascendActionReference.action;
        descendAction = descendActionReference.action;

        ascendAction.Enable();
        descendAction.Enable();

        ascendAction.started += StartAscend;
        ascendAction.canceled += StopAscend;
        descendAction.started += StartDescend;
        descendAction.canceled += StopDescend;
    }

    void OnDisable()
    {
        ascendAction.Disable();
        descendAction.Disable();
    }

    void Update()
    {
        InputMagnitude();

        finalMoveDirection += ascendMoveDirection * Time.deltaTime; // Add ascend/descend movement to final move direction
        finalMoveDirection += dashMoveDirection * Time.deltaTime; // Add dash movement to final move direction
        controller.Move(finalMoveDirection);
        finalMoveDirection = Vector3.zero; // Reset the final move direction after applying it
    }
    void InputMagnitude()
    {
        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player
        if (Speed > allowPlayerRotation)
        {
            // TODO: ANIMATION
            // animator.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            // TODO: ANIMATION
            // animator.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
        }
    }

    void PlayerMoveAndRotation()
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * InputZ + right * InputX;

        // Add the joystick input to the final move direction
        finalMoveDirection += desiredMoveDirection * Time.deltaTime * moveSpeed;
    }

    void LookInCameraDirection()
    {
        // Rotate the player towards the camera's forward direction
        Vector3 desiredRotationDirection = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        transform.rotation = Quaternion.LookRotation(desiredRotationDirection);
    }

    // Ascend and Descend methods
    void StartAscend(InputAction.CallbackContext context)
    {
        DOTween.To(() => ascendMoveDirection.y, x => ascendMoveDirection.y = x, ascendSpeed, 0.5f).SetEase(Ease.InOutQuad);
    }
    void StopAscend(InputAction.CallbackContext context)
    {
        DOTween.To(() => ascendMoveDirection.y, x => ascendMoveDirection.y = x, 0, 0.5f).SetEase(Ease.InOutQuad);
    }
    void StartDescend(InputAction.CallbackContext context)
    {
        DOTween.To(() => ascendMoveDirection.y, x => ascendMoveDirection.y = x, -ascendSpeed, 0.5f).SetEase(Ease.InOutQuad);
    }
    void StopDescend(InputAction.CallbackContext context)
    {
        DOTween.To(() => ascendMoveDirection.y, x => ascendMoveDirection.y = x, 0, 0.5f).SetEase(Ease.InOutQuad);
    }

    // Input actions
    void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        InputX = inputVector.x;
        InputZ = inputVector.y;
    }
    void OnDash(InputValue value)
    {
        if (value.isPressed && !isDashing)
        {
            isDashing = true;

            // Get the camera's forward and right vectors to adjust the dash direction
            var forward = cam.transform.forward;
            forward.y = 0; // Keep the dash horizontal
            forward.Normalize();
            var right = cam.transform.right;
            right.y = 0; // Keep the dash horizontal
            right.Normalize();

            Vector3 inputDirection = (forward * InputZ + right * InputX).normalized;

            // Dash the character in the direction they are facing or in the direction of movement
            Vector3 direction = Speed > allowPlayerRotation ? inputDirection : transform.forward;
            Vector3 target = direction * dashSpeed; // Calculate the dash movement vector

            dashMoveDirection = target;
            DOTween.To(() => dashMoveDirection, x => dashMoveDirection = x, Vector3.zero, 0.2f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => isDashing = false); // Reset the dash direction after the dash duration

            // TODO: ANIMATION
            // animator.SetTrigger("Dash");
        }
    }
    void OnAttack(InputValue value)
    {

    }
    void OnLook(InputValue value)
    {
        // TODO: After adding animations, if the player rotation looks weird, call this method in the update method instead
        LookInCameraDirection();
    }
}
