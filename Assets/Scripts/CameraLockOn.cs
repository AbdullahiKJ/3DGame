using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraLockOn : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera freeLookCam;
    [SerializeField] CinemachineCamera lockOnCam;
    [SerializeField] Transform cameraFollowTarget;
    [SerializeField] Animator cameraAnimator;
    Animator characterAnimator;
    [SerializeField] Image targetUI;
    [SerializeField] List<GameObject> visibleTargets;
    public bool isLockedOn = false;
    bool canSwitch = true;
    GameObject currentTarget;
    GameObject previousTarget;
    CinemachineInputAxisController camMovementScript;
    CinemachineOrbitalFollow freeLookCamOrbitalFollow;
    CinemachineTargetGroup targetGroup;

    void Awake()
    {
        // store camera movement script
        camMovementScript = FindFirstObjectByType<CinemachineInputAxisController>();
        targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
        characterAnimator = GetComponent<Animator>();
        freeLookCamOrbitalFollow = freeLookCam.GetComponent<CinemachineOrbitalFollow>();
    }

    void Update()
    {
        if(isLockedOn)
        {
            // display lock on target
            targetUI.enabled = true;
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(currentTarget.transform.position);
            Vector2 targetWorldPoint = new Vector2(1920 * viewportPosition.x, 1080 * viewportPosition.y);
            targetUI.gameObject.transform.position = targetWorldPoint;

            // make character look at target
            transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));
        }

        else
        {
            targetUI.enabled = false;
        }
    }

    // Add visible targets to the list
    public void AddTarget(GameObject newTarget)
    {
        visibleTargets.Add(newTarget);
    }
    
    // Check if a target is on the list
    public bool HasTarget(GameObject newTarget)
    {
        return visibleTargets.Contains(newTarget);
    }

    // Remove targets that are no longer visible
    public bool RemoveTarget(GameObject newTarget)
    {
        return visibleTargets.Remove(newTarget);
    }

    // check if target list is empty 
    bool isTargetAvailable()
    {
        return visibleTargets.Count != 0;
    }
    
    // get object closest to the camera
    GameObject GetClosestObject()
    {
        List<Vector3> objectPositions = new List<Vector3>();
        int closestObjectIndex = 0;

        foreach (GameObject target in visibleTargets)
        {
            // convert each target position to a new vector relative to the viewport
            objectPositions.Add(mainCamera.WorldToViewportPoint(target.transform.position));
        }

        for (int i = 0; i < objectPositions.Count; i++)
        {
            if (Mathf.Abs(objectPositions[i].x - 0.5f) < Mathf.Abs(objectPositions[closestObjectIndex].x - 0.5f))
            {
                closestObjectIndex = i;
            }
        }

        currentTarget = visibleTargets[closestObjectIndex];
        return visibleTargets[closestObjectIndex];
    }

    void OnLockOn(InputValue value)
    {
        if(value.isPressed)
        {
            if (isTargetAvailable() && !isLockedOn)
            {
                // set locked on status to true and change the animator value
                isLockedOn = true;
                characterAnimator.SetFloat("isLockedOn", 1);

                // get target closest to the middle of the camera
                GameObject target = GetClosestObject();

                // switch to lock on camera
                cameraAnimator.Play("LockOnCam");

                // set camera follow to target group
                lockOnCam.LookAt = target.transform;

                // disable camera movement script
                camMovementScript.enabled = false;
            }

            // if not locked on and no target ahead, turn camera to where the player is facing
            else if(!isLockedOn && !isTargetAvailable())
            {
                // turn on camera recentre
                freeLookCamOrbitalFollow.HorizontalAxis.Recentering.Enabled = true;
                freeLookCamOrbitalFollow.VerticalAxis.Recentering.Enabled = true;

                // turn off camera recentre
                StartCoroutine(WaitForCameraTurn());
            }
            
            else if(isLockedOn)
            {
                // reset locked on status to false and change aniamtor value
                isLockedOn = false;
                characterAnimator.SetFloat("isLockedOn", 0);

                // switch to free look camera
                cameraAnimator.Play("FreeLookCam");

                // Set the free look camera orientation and rotation to that of the lock on camera
                freeLookCam.ForceCameraPosition(lockOnCam.transform.position, lockOnCam.transform.rotation);

                // enable camera movement script
                camMovementScript.enabled = true;
            }   
        }       
    }

    void OnLook(InputValue value)
    {
        Vector2 playerInput = value.Get<Vector2>();
        if(isLockedOn && playerInput.magnitude > 0.5f && canSwitch)
        {
            // write function to place visible targets into a grid and move the lock on to another target based on the look input
            // create UI and place an indicator for the locked on target
            camMovementScript.enabled = false;
            lockOnCam.LookAt = CalculateAngles(playerInput).transform;
            StartCoroutine(WaitForLockSwitch());
        }
    }

    GameObject CalculateAngles(Vector2 playerInput)
    {
        List<Vector3> objectPositions = new List<Vector3>();
        List<float> objectAngles = new List<float>();
        List<GameObject> objectsWithinRange = new List<GameObject>();
        previousTarget = currentTarget;

        int closestObjectIndex = 0;

        foreach (GameObject target in visibleTargets)
        {
            // convert each target position to a new vector relative to the viewport
            objectPositions.Add(mainCamera.WorldToViewportPoint(target.transform.position));
        }

        // store the relative angles of the targets
        foreach(Vector3 position in objectPositions) 
        {
            objectAngles.Add(Mathf.Atan2(position.y-0.5f, position.x-0.5f) * Mathf.Rad2Deg);
        }

        // store the input angle
        float stickAngle = Mathf.Atan2(playerInput.y, playerInput.x) * Mathf.Rad2Deg;

        for(int i = 0; i < visibleTargets.Count; i++) 
        {
            if(visibleTargets[i] == currentTarget)
            {
                // ignore the current target when checking
            }

            // check if angle is less than a certain threshold
            else if(Mathf.Abs(objectAngles[i] - stickAngle) < 30f)
            {
                // closestObjectIndex = i;
                objectsWithinRange.Add(visibleTargets[i]);
            }
        }

        if(objectsWithinRange.Count > 0)
        {
            List<Vector3> newObjectsPositions = new List<Vector3>();
        foreach(GameObject target in objectsWithinRange)
        {
            // convert each target position to a new vector relative to the viewport
            newObjectsPositions.Add(mainCamera.WorldToViewportPoint(target.transform.position));
        }

        for(int i = 0; i < objectsWithinRange.Count; i++) 
        {
            Vector3 currentTargetViewPort = mainCamera.WorldToViewportPoint(currentTarget.transform.position);
            float currentDistance = new Vector2(newObjectsPositions[i].x - currentTargetViewPort.x, newObjectsPositions[i].y - currentTargetViewPort.y).magnitude;
            float previousDistance = new Vector2(newObjectsPositions[closestObjectIndex].x - currentTargetViewPort.x, newObjectsPositions[closestObjectIndex].y - currentTargetViewPort.y).magnitude;

            if(currentDistance < previousDistance)
            {
                closestObjectIndex = i;
            }
        }

        currentTarget = objectsWithinRange[closestObjectIndex];
        }
        
        if (previousTarget != currentTarget)
        {
            canSwitch = false;
        }
        return currentTarget;
    }

    IEnumerator WaitForLockSwitch()
    {
        yield return new WaitForSeconds(0.25f);
        canSwitch = true;
    }

    IEnumerator WaitForCameraTurn()
    {
        yield return new WaitForSeconds(freeLookCamOrbitalFollow.HorizontalAxis.Recentering.Wait + 0.5f);
        freeLookCamOrbitalFollow.HorizontalAxis.Recentering.Enabled = false;
        freeLookCamOrbitalFollow.VerticalAxis.Recentering.Enabled = false;
    }
}