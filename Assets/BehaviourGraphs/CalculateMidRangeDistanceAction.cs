using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateMidRangeDistance", story: "Use the [detector] script to update the [midRange] value", category: "Action", id: "c7c0e0089cb6881f34dd8419cbfe2bc5")]
public partial class CalculateMidRangeDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<Detector> Detector;
    [SerializeReference] public BlackboardVariable<float> MidRange;

    protected override Status OnStart()
    {
        float distance = Detector.Value.CheckWithinMidRange();
        if (distance >= 0f)
            MidRange.Value = distance;
        else
            MidRange.Value = 1f;
        return Status.Success;
    }
}

