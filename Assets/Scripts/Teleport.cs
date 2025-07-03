using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.UI.Shaders.Sample;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    [SerializeField] CinemachineCamera freeLookCam;
    [SerializeField] GameObject aimIndicator;
    [SerializeField] Vector2 defaultAxisGain;
    [SerializeField] float indicatorSpeed;

    public Camera mainCamera;
    CinemachineInputAxisController axisController;
    Movement movement;
    Vector2 moveInput;
    CharacterController controller;
    bool canTeleport = true;
    bool isTeleporting = false;
    bool isAiming = false;
    bool isSettingAimPosition = true;
    public float maxDistance = 100f;
    [SerializeField] float timeLimit = 5f;
    float timer = 0f;
    bool pointVisible = false;
    public List<GameObject> teleportTargets;
    GameObject currentTeleportTarget;
    [SerializeField] GameObject teleportMeter;
    Meter meterScript;
    Image meterImage;
    [SerializeField] Color disabledColour;
    [SerializeField] Color enabledColour;

    void Awake()
    {
        meterScript = teleportMeter.GetComponent<Meter>();
        meterImage = teleportMeter.GetComponent<Image>();

        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        aimIndicator.SetActive(false);
        axisController = freeLookCam.GetComponent<CinemachineInputAxisController>();

        SetAxisControllerGain(defaultAxisGain);
    }

    void Update()
    {
        // Move the aim indicator when aiming
        if (isAiming)
        {
            Vector3 movement = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f) * new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 newPosition = aimIndicator.transform.position + movement * indicatorSpeed;

            float distanceFromPlayer = (newPosition - transform.position).magnitude;
            if (distanceFromPlayer < maxDistance)
                aimIndicator.transform.position = newPosition;
        }

        // Ability timer
        if (!canTeleport && timer < timeLimit)
        {
            timer += Time.deltaTime;
            meterScript.Value = timer / timeLimit;
        }
        else
        {
            // todo: this is run every frame, use an event?
            canTeleport = true;
            timer = 0f;
            meterScript.Value = 1;
            meterImage.color = enabledColour;
        }
    }

    void OnTeleport(InputValue value)
    {
        // Ground teleport
        if (value.isPressed && isAiming && !movement.getIsRolling() && canTeleport)
            StartTeleport();

        // Platform teleport
        if (!isAiming && canTeleport)
        {
            pointVisible = TeleportPointsAvailable();
            Debug.Log(pointVisible);
            if (pointVisible)
                StartTeleport(true);
        }
    }

    void OnAim(InputValue value)
    {
        if (value.isPressed && !isTeleporting && !movement.getIsRolling())
        {
            isAiming = true;

            if (isSettingAimPosition)
            {
                SetAimPosition();
            }
        }
        else
        {
            isAiming = false;
            isSettingAimPosition = true;
            aimIndicator.SetActive(false);

            // Edit axis controller gain
            SetAxisControllerGain(defaultAxisGain);
        }
    }


    void OnLook(InputValue value)
    {
        if (isAiming)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    public bool getIsTeleporting()
    {
        return isTeleporting;
    }

    public bool getCanTeleport()
    {
        return canTeleport;
    }

    void StartTeleport(bool usingPlatform = false)
    {
        isTeleporting = true;

        // Update the teleport meter
        meterScript.Value = 0;
        meterImage.color = disabledColour;

        // todo create a coroutine to wait a certain amount of time

        // todo create and play an animation or activate a shader

        // Teleport to the raycast hit
        Vector3 newPosition;
        if (usingPlatform)
            newPosition = currentTeleportTarget.transform.position + new Vector3(0f, 2f, 0f) - transform.position;
        else
            newPosition = aimIndicator.transform.position + new Vector3(0f, 2f, 0f) - transform.position;

        controller.Move(newPosition);

        // Reset bools and variables
        canTeleport = false;
        currentTeleportTarget = null;

        // Deactivate the indicator
        aimIndicator.SetActive(false);
        isTeleporting = false;
    }

    void SetAimPosition()
    {
        // Edit axis controller gain
        SetAxisControllerGain(new Vector2(100f, -0.1f));

        // Set the indicator game object to active
        aimIndicator.SetActive(true);
        isSettingAimPosition = false;

        // Spawn the aim indicator
        Vector3 camForward = mainCamera.transform.forward;

        // Use only the x and y axes of the camera's forward rotation
        Vector3 spawnPoint = transform.position + new Vector3(camForward.x, 0f, camForward.z) * 5f;
        aimIndicator.transform.position = spawnPoint;
        return;
    }

    // Set the input axis gain for the free look camera
    void SetAxisControllerGain(Vector2 gain)
    {
        foreach (var c in axisController.Controllers)
        {
            if (c.Name == "Look Orbit X")
                c.Input.Gain = gain.x;
            else
                c.Input.Gain = gain.y;
        }
    }

    bool TeleportPointsAvailable()
    {
        if (teleportTargets.Count == 0)
            return false;

        // Get the object closest to the centre of the viewport
        currentTeleportTarget = Targets.Instance.GetClosestObject(teleportTargets);
        return true;
    }

}
