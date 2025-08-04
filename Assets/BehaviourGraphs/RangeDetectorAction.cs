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
    float checkInterval = 5f;
    float timer;

    protected override Status OnStart()
    {
        timer = checkInterval;
        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (timer < checkInterval)
            timer += Time.deltaTime;
        else
        {
            timer = 0f;
            CheckDistances();
        }
        return Status.Running;
    }

    void CheckDistances()
    {
        if (Detector.Value.CheckBaseAttackRange())
        {
            // If the player is within the base attack range, set the state to Close Range
            currentState.Value = State.CloseRange;
        }
        else if (Detector.Value.CheckWithinMidRange() >= 0f)
        {
            currentState.Value = State.MidRange;
        }
        else if (Detector.Value.CheckWithinLongRange())
        {
            currentState.Value = State.LongRange;
        }
        else if (currentState.Value == State.CloseRange || currentState.Value == State.MidRange
                || currentState.Value == State.LongRange)
        {
            // If the player does not satisfy the above attack conditions but is in any of the attack states, reset to Idle
            currentState.Value = State.Idle;
        }
    }
}