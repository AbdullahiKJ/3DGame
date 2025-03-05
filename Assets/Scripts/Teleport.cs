using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teleport : MonoBehaviour
{
    [SerializeField] float teleportSpeed = 5f;
    [SerializeField] LayerMask teleportLayer;
    [SerializeField] Animator cameraAnimator;
    [SerializeField] CinemachineCamera freeLookCam;
    [SerializeField] CinemachineCamera aimCam;

    public Camera mainCamera;
    Movement movement;
    bool isTeleporting = false;
    bool isAiming = false;
    float maxDistance = 100f;
    // DEBUG
    RaycastHit hit;
    bool hitFound = false;

    void Awake()
    {
        movement = GetComponent<Movement>();
    }

    void Update()
    {

    }

    void LateUpdate()
    {

    }

    // DEBUG
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitFound)
            Gizmos.DrawSphere(transform.position, 0.25f);
    }

    void OnTeleport(InputValue value)
    {
        if (value.isPressed && !movement.getIsRolling())
        {
            isTeleporting = true;
            StartTeleport();
        }
        else
        {
            StopTeleport();
        }
    }

    void OnAim(InputValue value)
    {
        if (value.isPressed && !isTeleporting && !movement.getIsRolling())
        {
            cameraAnimator.Play("AimCam");
            isAiming = true;

            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, maxDistance, teleportLayer))
            {
                hitFound = true;
            }

            // Set the free look camera orientation and rotation to that of the lock on camera
            freeLookCam.ForceCameraPosition(aimCam.transform.position, aimCam.transform.rotation);
        }
        else
        {
            cameraAnimator.Play("FreeLookCam");
            isAiming = false;

            // Set the free look camera orientation and rotation to that of the lock on camera
            aimCam.ForceCameraPosition(freeLookCam.transform.position, freeLookCam.transform.rotation);
        }
    }

    public bool getIsTeleporting()
    {
        return isTeleporting;
    }

    void StartTeleport()
    {
        if (hitFound == true)
        {
            // Use coroutine to wait a certain amount of time

            // Teleport to the raycast hit
            transform.position = hit.point;
        }
    }

    void StopTeleport()
    {
        isTeleporting = false;
    }

}
