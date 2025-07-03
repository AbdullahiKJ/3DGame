using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetector", story: "Use [Detector] to update the [currentState]", category: "Action", id: "695e378e84706e46c9171c68c08bbc7f")]
public partial class RangeDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<Detector> Detector;
    [SerializeReference] public BlackboardVariable<State> currentState;

    protected override Status OnUpdate()
    {
        if (Detector.Value.CheckBaseAttackRange())
        {
            // If the player is within the base attack range, set the state to Attacking
            currentState.Value = State.Attacking;
        }
        else if (currentState.Value == State.Attacking)
        {
            // If the player is not within the base attack range and the current state is Attacking, set the state to Idle
            currentState.Value = State.Idle;
        }
        return Status.Running;
    }
}