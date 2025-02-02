using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stretch : MonoBehaviour
{
    [SerializeField] GameObject hand;
    LineRenderer lineRenderer;
    [SerializeField] float swingSpeed = 5f;
    [SerializeField] LayerMask swingLayer;

    public Camera mainCamera;
    CharacterController controller;
    public bool isSwinging = false;
    Vector3 swingPoint;
    Vector3 distance;
    float maxDistance = 100f;
    SpringJoint springJoint;
    Vector3 currentSwingPosition;
    // DEBUG
    RaycastHit hit;
    bool hitFound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        lineRenderer = hand.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwinging)
        {

        }
    }

    void LateUpdate()
    {
        Debug.Log(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, maxDistance, swingLayer));
        if (isSwinging)
        {
            DrawRope();
        }
    }

    // DEBUG
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitFound)
            Gizmos.DrawSphere(transform.position, 0.25f);
    }

    void OnSwing(InputValue value)
    {
        if (value.isPressed && !isSwinging)
        {
            Debug.Log(isSwinging);
            isSwinging = true;
            StartSwing();
        }
        else if (isSwinging)
        {
            isSwinging = false;
            StopSwing();
        }
    }

    void DrawRope()
    {
        if (!springJoint)
            return;

        Debug.Log("isSwinging");
        currentSwingPosition = Vector3.Lerp(currentSwingPosition, swingPoint, Time.deltaTime * 8f);

        lineRenderer.SetPosition(0, hand.transform.position);
        lineRenderer.SetPosition(1, currentSwingPosition);
    }

    void StartSwing()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, maxDistance, swingLayer))
        {

            // DEBUG
            hitFound = true;
            Debug.Log("found target");
            swingPoint = hit.point;
            springJoint = hand.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(hand.transform.position, swingPoint);

            // Distance maintained between the player and swing point
            springJoint.maxDistance = distanceFromPoint * 0.8f;
            springJoint.minDistance = distanceFromPoint * 0.25f;

            // Spring parameters
            springJoint.spring = 4.5f;
            springJoint.damper = 7f;
            springJoint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            currentSwingPosition = hand.transform.position;
        }
    }

    void StopSwing()
    {
        lineRenderer.positionCount = 0;
        Destroy(springJoint);
    }

}
