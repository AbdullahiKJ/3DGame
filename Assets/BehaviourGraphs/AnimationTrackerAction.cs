using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AnimationTracker", story: "Wait for current [animator] animation to end", category: "Action", id: "f2af2e08a8f1d14de816cc5df93d4c2b")]
public partial class AnimationTrackerAction : Action
{
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    bool isInTransition = true;
    bool timerStarted = false;
    Animator animator;
    float timer = 0f;
    float animDuration = 0f;

    protected override Status OnStart()
    {
        animator = Animator.Value;
        isInTransition = animator.IsInTransition(0);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (isInTransition)
        {
            // Check if the animator is still transitioning
            isInTransition = animator.IsInTransition(0);
        }
        else if (!timerStarted)
        {
            // Get current animation playing
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            // Get the animation length and play time
            animDuration = currentState.length;
            timer = animDuration * currentState.normalizedTime;
            timerStarted = true;
        }
        // Check if the animation is still playing
        else
        {
            if (timer > animDuration)
            {
                ResetVariables();
                return Status.Success;
            }
            else
                timer += Time.deltaTime;
        }
        return Status.Running;
    }
    void ResetVariables()
    {
        isInTransition = true;
        timerStarted = false;
        timer = 0f;
        animDuration = 0f;
    }
}

