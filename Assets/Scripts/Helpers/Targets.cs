using System.Collections.Generic;
using UnityEngine;

public class Targets : MonoBehaviour
{
    Camera mainCamera;
    public static Targets Instance;

    void Awake()
    {
        Instance = this;
        mainCamera = FindFirstObjectByType<Camera>();
    }

    // Add visible targets to the list
    public void AddTarget(GameObject newTarget, List<GameObject> targetList)
    {
        targetList.Add(newTarget);
    }

    // Check if a target is on the list
    public bool HasTarget(GameObject newTarget, List<GameObject> targetList)
    {
        return targetList.Contains(newTarget);
    }

    // Remove targets that are no longer visible
    public bool RemoveTarget(GameObject newTarget, List<GameObject> targetList)
    {
        return targetList.Remove(newTarget);
    }

    // Get the game object closest to the centre of the viewport
    public GameObject GetClosestObject(List<GameObject> targetList)
    {
        List<Vector3> objectPositions = new List<Vector3>();
        int closestObjectIndex = 0;

        foreach (GameObject target in targetList)
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

        return targetList[closestObjectIndex];
    }
}
