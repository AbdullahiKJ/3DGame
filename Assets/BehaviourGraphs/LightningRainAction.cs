using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LightningRain", story: "play [lightningVFX], [ambientClip] and [lightningStrike] at the [agent] position towards the [target] position", category: "Action", id: "a74796651db8c457c578ae0cb1949d22")]
public partial class LightningRainAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> lightningVFX;
    [SerializeReference] public BlackboardVariable<AudioClip> ambientClip;
    [SerializeReference] public BlackboardVariable<GameObject> lightningStrike;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;
    Animator agentAnimator;
    float timer = 0f;
    float abilityDuration = 30f; // Duration of the lightning rain ability
    bool startVFX = false; // Flag to indicate if the VFX has started
    float castDuration = 5f; // Animation duration gotten directly from the clip
    GameObject vfxInstance;
    GameObject strikeInstance;

    protected override Status OnStart()
    {
        agentAnimator = agent.Value.GetComponent<Animator>();
        agentAnimator.Play("Cast Lightning");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (timer < abilityDuration)
        {
            timer += Time.deltaTime;
        }
        else
        {
            // If the ability duration has passed, destroy any existing VFX instances
            if (vfxInstance != null)
            {
                // Destroy the VFX instance when the action ends
                GameObject.Destroy(vfxInstance);
            }
            if (strikeInstance != null)
            {
                // Destroy any existing child instances from the lightning strike
                strikeInstance.GetComponent<TrackingLightning>().DestroyInstances();
                // Destroy the lightning strike instance when the action ends
                GameObject.Destroy(strikeInstance);
            }
            return Status.Success;
        }
        CheckCastTimer();
        return Status.Running;
    }

    void CheckCastTimer()
    {
        if (startVFX)
        {
            // Do nothing if the VFX has already started
        }
        else if (timer >= castDuration)
        {
            // Instantiate the lightning VFX at the agent's position
            vfxInstance = GameObject.Instantiate(lightningVFX.Value, agent.Value.transform.position, Quaternion.identity);

            // Instantiate the lightning strike at the origin and assign the target transform
            strikeInstance = GameObject.Instantiate(lightningStrike.Value, Vector3.zero, Quaternion.identity);
            strikeInstance.GetComponent<TrackingLightning>().targetTransform = target.Value.transform;

            // Play the ambient sound
            SoundFXManager.instance.PlayAmbientClip(ambientClip, agent.Value.transform, 1f, abilityDuration);

            startVFX = true; // Set flag to true to indicate VFX has started
        }
    }
}

