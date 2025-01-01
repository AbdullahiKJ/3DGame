using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stretch : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public Camera mainCamera;
    [SerializeField] List<MeshRenderer> visibleTargets;
    Vector3 currentTarget;
    CharacterController controller;
    bool isSwinging = false;
    Vector3 distance;
    [SerializeField] float swingSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwinging)
        {
            distance =  currentTarget - transform.position;
            controller.Move(distance.normalized * swingSpeed);
            if (distance.magnitude < 0.2f)
            {
                isSwinging = false;
                lineRenderer.positionCount = 0;
                controller.Move(Vector3.zero);
            }
        }
    }

    void LateUpdate()
    {
        if (isSwinging)
        {
            drawRope();
        }
    }

    void OnSwing(InputValue value)
    {
        if(value.isPressed && visibleTargets.Count > 0)
        {
            currentTarget = visibleTargets[0].transform.position;
            isSwinging = true;
        }
    }

    void drawRope() 
    {
        lineRenderer.SetPosition(0, lineRenderer.gameObject.transform.position);
        lineRenderer.SetPosition(1, currentTarget);
    }

    // Add visible targets to the list
    public void AddTarget(MeshRenderer newTarget)
    {
        visibleTargets.Add(newTarget);
    }
    
    // Check if a target is on the list
    public bool HasTarget(MeshRenderer newTarget)
    {
        return visibleTargets.Contains(newTarget);
    }

    // Remove targets that are no longer visible
    public void RemoveTarget(MeshRenderer newTarget)
    {
        visibleTargets.Remove(newTarget);
    }
}
