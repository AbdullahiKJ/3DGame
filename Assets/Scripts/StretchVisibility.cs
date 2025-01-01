using UnityEngine;

public class StretchVisibility : MonoBehaviour
{
    Stretch stretch;
    MeshRenderer stretchPoint;
    void Awake()
    {
        stretch = FindFirstObjectByType<Stretch>();  
        stretchPoint = gameObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Vector3 camPosition = Camera.main.WorldToViewportPoint(this.transform.position);
        if (camPosition.x >= 0f && camPosition.x <= 1f && camPosition.y >= 0f && camPosition.y <= 1f && camPosition.z > 0f) {
            if(!stretch.HasTarget(stretchPoint) )
            {
                stretch.AddTarget(stretchPoint);
            }
        }
        else {
            if(stretch.HasTarget(stretchPoint))
            {
                stretch.RemoveTarget(stretchPoint);
            }
        }
    }

    // run when object comes into view of camera
    // void OnBecameVisible()
    // {
    //     Debug.Log("visible");
    //     if(!stretch.HasTarget(stretchPoint) )
    //     {
    //         stretch.AddTarget(stretchPoint);
    //     }
    // }

    // void OnBecameInvisible()
    // {
    //     Debug.Log("invisible");
    //     if(stretch.HasTarget(stretchPoint))
    //     {
    //         stretch.RemoveTarget(stretchPoint);
    //     }
    // }
}
