using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Meteor", story: "play [meteorRainPrefab] at the [agent] position towards the [target] position and set [meteorFlag]", category: "Action", id: "b7553e04b1ad595b8202a49ce2dbdd43")]
public partial class MeteorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> meteorRainPrefab;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;
    [SerializeReference] public BlackboardVariable<bool> meteorFlag;

    protected override Status OnStart()
    {
        GameObject instance = (GameObject)GameObject.Instantiate(meteorRainPrefab, agent.Value.transform.position, Quaternion.identity);
        instance.GetComponent<Meteors>().meteorAction = this; // Set the meteor action reference
        meteorFlag.Value = true; // Set the meteor flag to true
        return Status.Success;
    }
}

