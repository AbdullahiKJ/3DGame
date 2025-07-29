using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

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

    protected override Status OnStart()
    {
        // Play the agent animation
        if (agent.Value.TryGetComponent<Animator>(out Animator agentAnimator))
        {
            agentAnimator.Play("Battlecry");
        }

        // Push the target away from the agent if they are close enough
        // TODO: evaluate if this should apply when on platforms
        posDiff = target.Value.transform.position - agent.Value.transform.position;
        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        // Only start pushing after a delay
        if (timer >= startTime && !pushStarted)
        {
            pushStarted = true;
            if (posDiff.magnitude < pushDistance)
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
            target.Value.transform.LookAt(lookatTarget);

        // Check if the push is completed
        if (pushCompleted)
        {
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


