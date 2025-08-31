using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GenerateRandomInt", story: "Generate a random integer between 0 and [maxValue] and assign it to the [target] variable", category: "Action", id: "d80c7be866f0c5368fe712249e251880")]
public partial class GenerateRandomIntAction : Action
{
    [SerializeReference] public BlackboardVariable<int> MaxValue;
    [SerializeReference] public BlackboardVariable<int> Target;

    protected override Status OnStart()
    {
        Target.Value = UnityEngine.Random.Range(0, MaxValue.Value + 1);
        return Status.Success;
    }
}

