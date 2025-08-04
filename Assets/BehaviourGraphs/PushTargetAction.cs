using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;
using UnityEngine.InputSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "pushTarget", story: "push the [target] away from [agent] and apply [animator] framing", category: "Action", id: "21568f0c16340087efe7e2d78bf09844")]
public partial class PushTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> target;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<Animator> animator;
    float pushDuration = 4f;
    float pushDistance = 50f;
    bool pushCompleted = false;
    Vector3 posDiff;
    bool pushStarted = false;
    float timer = 0f;
    float startTime = 1.5f;
    Vector3 lookatTarget;
    PlayerInput targetInput;
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

        // Get the target's input system
        targetInput = target.Value.GetComponent<PlayerInput>();
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
                // Make the target look at the agent
                lookatTarget = agent.Value.transform.position;
                lookatTarget.y = target.Value.transform.position.y; // Ignore y-axis for rotation
                target.Value.transform.LookAt(lookatTarget);

                if (target.Value.TryGetComponent<Animator>(out Animator targetAnimator))
                {
                    targetAnimator.Play("Block");
                }
                // Frame the player and target in the camera view
                animator.Value.Play("EnemyLook");

                // Push the target away from the agent
                DOTween.To(() => target.Value.transform.position,
                    (newPos) => target.Value.transform.position = newPos,
                    target.Value.transform.position + posDiff.normalized * pushDistance,
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

        if (pushStarted)
        {
            // Disable the target input system and look at the agent
            targetInput.enabled = false;
            target.Value.transform.LookAt(lookatTarget);
        }

        // Check if the push is completed
        if (pushCompleted)
        {
            targetInput.enabled = true;
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


