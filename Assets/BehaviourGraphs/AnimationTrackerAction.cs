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

    protected override Status OnUpdate()
    {
        // Get current animation playing
        Animator animator = Animator.Value;
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // Get the current animation time normalized (0 to 1)
        float currentAnimationTime = currentState.normalizedTime;

        // Continue if the current state is the blend tree or the animation has not finished
        if (currentState.IsName("Movement Blend Tree") || currentAnimationTime < 1f)
        {
            // Attack animation has not started yet
            return Status.Running;
        }


        // Stop if the animation is finished
        else
        {
            // Animation is finished
            return Status.Success;
        }
    }
}

