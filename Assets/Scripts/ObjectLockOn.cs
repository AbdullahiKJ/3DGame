using UnityEngine;

public class ObjectLockOn : MonoBehaviour
{
    CameraLockOn cameraLockOn;
    void Awake()
    {
        cameraLockOn = FindFirstObjectByType<CameraLockOn>();
    }

    // run when object comes into view of camera
    void OnBecameVisible()
    {
        // check if in list and add
        if (!Targets.Instance.HasTarget(gameObject, cameraLockOn.visibleTargets))
        {
            Targets.Instance.AddTarget(gameObject, cameraLockOn.visibleTargets);
        }
    }

    void OnBecameInvisible()
    {
        if (Targets.Instance.HasTarget(gameObject, cameraLockOn.visibleTargets))
        {
            Targets.Instance.RemoveTarget(gameObject, cameraLockOn.visibleTargets);
        }
    }
}
