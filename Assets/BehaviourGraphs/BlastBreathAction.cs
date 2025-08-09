using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.VFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BlastBreath", story: "play [charge], [chargeAudio], [ambientSound], [blastAudio] and [blastBreath] VFX at the [agent] position towards the [target] position", category: "Action", id: "dd72fc417619b909e83bd51bf5586bc7")]
public partial class BlastBreathAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> charge;
    [SerializeReference] public BlackboardVariable<AudioClip> chargeAudio;
    [SerializeReference] public BlackboardVariable<AudioClip> ambientSound;
    [SerializeReference] public BlackboardVariable<AudioClip> blastAudio;
    [SerializeReference] public BlackboardVariable<GameObject> blastBreath;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;
    GameObject chargeInstance;
    GameObject blastBreathInstance;
    CapsuleCollider collider;
    float chargeDuration;
    float blastBreathDuration;
    float colliderLifeDuration = 0.15f; // Duration of the prefabs z axis elongation from the vfx graph
    float colliderDuration;
    float blastBreathScale = 45f; // Scale from the vfx, size is ~49 but we want it to be larger so it goes past the target
    float colliderScaleCorrection = 1.1f; // To ensure the collider matches the blast breath VFX size
    float distanceToTarget;
    float timer = 0f;
    bool isCharging = true;

    protected override Status OnStart()
    {
        // Instantiate vfx prefabs at the agent's position
        chargeInstance = GameObject.Instantiate(charge.Value, agent.Value.transform);

        // Get the charge and blast breath durations
        chargeDuration = chargeInstance.GetComponent<VisualEffect>().GetFloat("Lifetime");

        // Play the charge audio
        SoundFXManager.instance.PlaySoundFXClip(chargeAudio, agent.Value.transform, 1f, chargeDuration);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if the charge animation is still playing
        if (isCharging)
        {
            // Look at the target while charging (ignore the y-axis)
            Vector3 lookatTarget = target.Value.transform.position;
            lookatTarget.y = agent.Value.transform.position.y; // Ignore y-axis for rotation
            agent.Value.transform.LookAt(lookatTarget);

            // Update the isCharging flag based on the timer
            CheckChargingTimer();
        }
        else
        {
            // Check if the blast breath animation is still playing and adjust the collider scale/position
            return CheckBreathTimer();
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (chargeInstance != null)
        {
            GameObject.Destroy(chargeInstance);
        }
        if (blastBreathInstance != null)
        {
            GameObject.Destroy(blastBreathInstance);
        }
    }

    void CheckChargingTimer()
    {
        if (timer >= chargeDuration)
        {
            // If the charge animation has ended, set isCharging to false
            isCharging = false;

            // Get the distance to the target
            distanceToTarget = Vector3.Distance(agent.Value.transform.position, target.Value.transform.position);

            // Instantiate the blast breath VFX at the agent's position and get it's duration and collider
            blastBreathInstance = GameObject.Instantiate(blastBreath.Value, agent.Value.transform.position, Quaternion.identity);
            collider = blastBreathInstance.GetComponent<CapsuleCollider>();
            blastBreathDuration = blastBreathInstance.GetComponent<VisualEffect>().GetFloat("Lifetime");
            colliderDuration = blastBreathDuration * colliderLifeDuration;

            // Play the blast breath audio and ambient
            SoundFXManager.instance.PlaySoundFXClip(blastAudio, agent.Value.transform, 1f);
            SoundFXManager.instance.PlayAmbientClip(ambientSound, agent.Value.transform, 1f, blastBreathDuration);

            // Set the position, scale and orientation of the blast breath instance towards the target
            blastBreathInstance.transform.LookAt(target.Value.transform);
            blastBreathInstance.transform.localScale = new Vector3(1f, 1f, distanceToTarget / blastBreathScale);

            // Reset the timer for the blast breath
            timer = 0f;

            // Play the agent animation
            if (agent.Value.TryGetComponent<Animator>(out Animator agentAnimator))
            {
                agentAnimator.Play("Blast Breath Scream");
            }
        }
    }

    Status CheckBreathTimer()
    {
        if (timer >= blastBreathDuration)
        {
            // If the blast breath animation has ended, end the action
            return Status.Success;
        }
        // Update the scale and positon of the collider
        else
        {
            UpdateColliderScale();
            return Status.Running;
        }
    }

    void UpdateColliderScale()
    {
        if (timer <= colliderDuration)
        {
            float currentSize = (timer / colliderDuration) * distanceToTarget;
            // Bypass the colliders height scale adjustment
            collider.height = currentSize / blastBreathInstance.transform.localScale.z * colliderScaleCorrection;
            collider.center = new Vector3(0f, 0f, collider.height / 2f);
        }
    }
}

