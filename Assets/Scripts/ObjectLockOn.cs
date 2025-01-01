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
        if(!cameraLockOn.HasTarget(gameObject))
        {
            cameraLockOn.AddTarget(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        if(cameraLockOn.HasTarget(gameObject))
        {
            cameraLockOn.RemoveTarget(gameObject);
        }
    }
}
