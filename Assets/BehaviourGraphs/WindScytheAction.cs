using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.VFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "windScythe", story: "play [tornado] and [scythe] VFX at the [agent] position towards the [target] position", category: "Action", id: "08a69cdb2f8c516afd3165a5ddae7308")]
public partial class WindScytheAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Tornado;
    [SerializeReference] public BlackboardVariable<GameObject> Scythe;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;
    GameObject tornadoInstance;
    float timer = 0f;
    float tornadoDuration = 30f;
    float scytheDelay = 5f;

    protected override Status OnStart()
    {
        tornadoInstance = GameObject.Instantiate(Tornado.Value, agent.Value.transform.position, Quaternion.identity);

        // Assign the target to the particle aim script
        ParticleAim[] particleAim = tornadoInstance.GetComponentsInChildren<ParticleAim>();
        foreach (ParticleAim script in particleAim)
        {
            script.target = target.Value.transform;
        }

        // Get the duration of the tornado effect
        if (tornadoInstance.TryGetComponent<VisualEffect>(out VisualEffect tornadoVFX))
        {
            tornadoDuration = tornadoVFX.GetFloat("Lifetime");
        }
        else
        {
            tornadoDuration = 30f; // Default duration if not found
        }

        // Set the duration for the particle system
        ParticleSystem scytheParticle = tornadoInstance.transform.Find("Directional Scythe").GetComponent<ParticleSystem>();
        if (scytheParticle != null)
        {
            var main = scytheParticle.main;
            main.duration = tornadoDuration - scytheDelay;
            main.startDelay = scytheDelay;
            scytheParticle.Play();
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (timer < tornadoDuration)
        {
            timer += Time.deltaTime;
        }
        else
        {
            // Destroy the tornado instance after its duration
            if (tornadoInstance != null)
            {
                GameObject.Destroy(tornadoInstance);
            }
            return Status.Success; // End the action after the scythe is instantiated
        }
        return Status.Success;
    }
}

