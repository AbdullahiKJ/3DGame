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
    [SerializeField] GameObject dashTrail;
    TrailRenderer[] trailObjects;
    Gradient oldGradient;
    [SerializeField] AudioClip[] dashSFX;

    [Header("Ascend/Descend")]

    [SerializeField] InputActionReference ascendActionReference;
    [SerializeField] InputActionReference descendActionReference;
    InputAction ascendAction;
    InputAction descendAction;
    public float ascendSpeed = 5f;
    Vector3 ascendMoveDirection = Vector3.zero;

    [Header("Lightning Projectile")]
    [SerializeField] GameObject projectilePrefab;

    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject leftHand;
    GameObject rightInstance;
    GameObject leftInstance;
    float projectileDistance;
    float combo = 0;

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

        // Get the dash trail and its components
        trailObjects = dashTrail.GetComponentsInChildren<TrailRenderer>();
        oldGradient = trailObjects[0].colorGradient;

        // Disable the dash trail by default
        dashTrail.SetActive(false);

        // Get the projectileDistance
        projectileDistance = projectilePrefab.GetComponent<ProjectileDamage>().maxFlyDistance;
    }

    void OnDisable()
    {
        ascendAction.Disable();
        descendAction.Disable();
    }

    void Update()
    {
        InputMagnitude();
        // TOOD: after adding animations, check if this is needed
        LookInCameraDirection();

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
            // animator.SetFloat("VerticalInput", InputZ, StartAnimTime, Time.deltaTime);
            // animator.SetFloat("HorizontalInput", InputX, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            // TODO: ANIMATION
            animator.SetFloat("VerticalInput", InputZ, StopAnimTime, Time.deltaTime);
            animator.SetFloat("HorizontalInput", InputX, StopAnimTime, Time.deltaTime);
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

    // Fade out the trail alpha
    void FadeTrailAlpha()
    {
        float alpha = 1f;
        float fadeDuration = trailObjects[0].time; // Duration for the fade out

        DOTween.To(() => alpha, a =>
        {
            alpha = a;

            // Create a new gradient to update alpha
            Gradient newGradient = new Gradient();
            GradientColorKey[] colorKeys = oldGradient.colorKeys;
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colorKeys.Length];

            for (int i = 0; i < colorKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(alpha, colorKeys[i].time);
            }

            newGradient.SetKeys(colorKeys, alphaKeys);

            foreach (TrailRenderer trail in trailObjects)
            {
                // Update the color gradient of each trail renderer
                trail.colorGradient = newGradient;
            }
        }, 0f, fadeDuration)
        .OnComplete(() =>
        {
            dashTrail.SetActive(false); // disable after fade
        });
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

            // Play the sound effect
            SoundFXManager.instance.PlayLayeredSoundFX(dashSFX, transform, 1f);

            // Enable the dash trail effect
            dashTrail.SetActive(true);

            // Reset the dash color gradient to full alpha
            foreach (TrailRenderer trail in trailObjects)
            {
                // Reset the color gradient to the original one
                trail.colorGradient = oldGradient;
            }

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
                .OnComplete(() =>
                {
                    isDashing = false; // Reset the dash direction after the dash duration
                    FadeTrailAlpha(); // Disable the dash trail after the fading the alpha
                });

            // TODO: ANIMATION
            // animator.SetTrigger("Dash");
        }
    }
    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            // TODO: throw animation
            // todo: wait for throw to complete before creating new instance
            // todo: wait for throw to complete before you can throw again
            RaycastHit hit;
            Vector3 target;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            int layerMask = ~LayerMask.GetMask("Player");
            // ~ inverts the mask so all layers except these are hit.

            // Cast a ray from the centre of the screen to get the projectile target
            if (Physics.Raycast(ray, out hit, projectileDistance, layerMask))
            {
                target = hit.point;
            }
            // Use the projectile distance if there is no hit
            else
            {
                target = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, projectileDistance));
            }

            // Launch the projectile
            if (combo == 0)
            {
                rightInstance = Instantiate(projectilePrefab, rightHand.transform.position, Quaternion.identity);
                rightInstance.GetComponent<ProjectileDamage>().Launch(target, transform.position);
                combo = 1;
            }
            else
            {
                leftInstance = Instantiate(projectilePrefab, leftHand.transform.position, Quaternion.identity);
                leftInstance.GetComponent<ProjectileDamage>().Launch(target, transform.position);
                combo = 0;
            }
        }
    }
    void OnLook(InputValue value)
    {
        // TODO: After adding animations, if the player rotation looks weird, call this method in the update method instead
        // TODO: also this was getting called even when this script was disabled
        // LookInCameraDirection();
    }
}
