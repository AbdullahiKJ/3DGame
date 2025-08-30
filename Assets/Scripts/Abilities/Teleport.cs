using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering;

public class Teleport : AbilityBase
{
    [Header("Teleport Settings")]
    [SerializeField] CinemachineCamera freeLookCam;
    [SerializeField] GameObject aimIndicator;
    [SerializeField] Vector2 defaultAxisGain;
    [SerializeField] float indicatorSpeed;

    public Camera mainCamera;
    CinemachineInputAxisController axisController;
    Movement movement;
    Vector2 moveInput;
    CharacterController controller;
    bool isAiming = false;
    bool isSettingAimPosition = true;
    public float maxDistance = 100f;
    bool pointVisible = false;
    public List<GameObject> teleportTargets;
    GameObject currentTeleportTarget;
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] Volume teleportVolume;
    [SerializeField] AudioClip[] soundFX;

    void Awake()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        aimIndicator.SetActive(false);
        axisController = freeLookCam.GetComponent<CinemachineInputAxisController>();

        SetAxisControllerGain(defaultAxisGain);
    }

    public override void Ability()
    {
        // Ground teleport
        if (isAiming && !movement.getIsRolling())
            StartCoroutine(WaitForTeleportVfx());

        // Platform teleport
        if (!isAiming)
        {
            pointVisible = TeleportPointsAvailable();
            if (pointVisible)
                StartCoroutine(WaitForTeleportVfx(true));
        }
    }

    public override void Helper()
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
    }

    public override void EndAbility()
    {
    }


    void OnAim(InputValue value)
    {
        if (value.isPressed && !abilityStarted && !movement.getIsRolling())
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

    // todo: teleport points are not appearing behind objects
    // todo: disable teleport points close to the player: visually and the game object as well
    void OnLook(InputValue value)
    {
        if (isAiming)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    IEnumerator WaitForTeleportVfx(bool usingPlatform = false, bool callTeleport = true)
    {
        abilityStarted = true;
        GameObject vfxInstance = Instantiate(vfxPrefab, transform);

        // Play sound effects
        if (callTeleport)
            SoundFXManager.instance.PlayLayeredSoundFX(soundFX, transform, 1f);

        yield return new WaitForSecondsRealtime(0.5f);

        if (callTeleport)
            StartTeleport(usingPlatform);

        // Destroy the teleport VFX instance
        if (vfxInstance != null)
            Destroy(vfxInstance);

    }
    void StartTeleport(bool usingPlatform = false)
    {
        // Teleport to the raycast hit
        Vector3 newPosition;
        if (usingPlatform)
            newPosition = currentTeleportTarget.transform.position + new Vector3(0f, 2f, 0f) - transform.position;
        else
            newPosition = aimIndicator.transform.position + new Vector3(0f, 2f, 0f) - transform.position;

        controller.Move(newPosition);

        float transitionLength = 0.35f;
        DOTween.To(() => teleportVolume.weight, x => teleportVolume.weight = x, 1f, transitionLength)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => DOTween.To(() => teleportVolume.weight, x => teleportVolume.weight = x, 0f, transitionLength).SetEase(Ease.InOutQuad));

        // Play teleport VFX again
        StartCoroutine(WaitForTeleportVfx(false, false));

        // Reset teleport target
        currentTeleportTarget = null;

        // Deactivate the indicator
        aimIndicator.SetActive(false);
        abilityStarted = false;
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
