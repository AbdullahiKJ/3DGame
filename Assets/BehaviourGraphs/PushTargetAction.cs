using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;
using UnityEngine.InputSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "pushTarget", story: "push the [target] away from [agent] and apply [animator] framing and [cinemaScope]", category: "Action", id: "21568f0c16340087efe7e2d78bf09844")]
public partial class PushTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> target;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<Animator> animator;
    [SerializeReference] public BlackboardVariable<Cinemascope> cinemaScope;
    float pushDuration = 4f;
    float pushDistance = 75f;
    bool pushCompleted = false;
    Vector3 posDiff;
    bool pushStarted = false;
    float timer = 0f;
    float startTime = 1.5f;
    Vector3 lookatTarget;
    PlayerInput targetInput;
    CameraLockOn cameraLockOn;
    float minHeightDifference = 10f;

    protected override Status OnStart()
    {
        // Play the agent animation
        if (agent.Value.TryGetComponent<Animator>(out Animator agentAnimator))
        {
            agentAnimator.Play("Battlecry");
        }

        // Push the target away from the agent if they are close enough
        posDiff = target.Value.transform.position - agent.Value.transform.position;

        // Get the target's input system and camera lock on script
        targetInput = target.Value.GetComponent<PlayerInput>();
        cameraLockOn = target.Value.GetComponent<CameraLockOn>();

        // Disable the target input system and look at the agent
        targetInput.enabled = false;
        lookatTarget = agent.Value.transform.position;
        lookatTarget.y = target.Value.transform.position.y; // Ignore y-axis for rotation
        target.Value.transform.LookAt(lookatTarget);

        // Reset the target's camera if locked on
        cameraLockOn.ResetCamera();

        // Start the cinemascope transition
        cinemaScope.Value.ShowBars();

        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        // Only start pushing after a delay
        if (timer >= startTime && !pushStarted)
        {
            pushStarted = true;
            float heightDifference = Math.Abs(target.Value.transform.position.y - agent.Value.transform.position.y);
            if (posDiff.magnitude < pushDistance && heightDifference < minHeightDifference)
            {
                if (target.Value.TryGetComponent<Animator>(out Animator targetAnimator))
                {
                    targetAnimator.Play("Block");
                }

                // Frame the player and target in the camera view
                animator.Value.Play("EnemyLook");

                // Push the target away from the agent
                float pushAmount = pushDistance - posDiff.magnitude;
                DOTween.To(() => target.Value.transform.position,
                    (newPos) => target.Value.transform.position = newPos,
                    target.Value.transform.position + posDiff.normalized * pushAmount,
                    pushDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        ResetTargetCamera();
                    });
            }
        }
        else if (timer > startTime + pushDuration)
        {
            // If the push is completed, reset the state
            ResetTargetCamera();
        }
        else
        {
            timer += Time.deltaTime;
        }

        // Check if the push is completed
        if (pushCompleted)
        {
            targetInput.enabled = true;
            cinemaScope.Value.HideBars();
            return Status.Success;
        }

        return Status.Running;
    }
    void ResetTargetCamera()
    {
        pushCompleted = true;
        animator.Value.Play("FreeLookCam");
    }
}
