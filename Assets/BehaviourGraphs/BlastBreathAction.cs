using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.VFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BlastBreath", story: "play [charge] and [blastBreath] VFX at the [agent] position towards the [target] position", category: "Action", id: "dd72fc417619b909e83bd51bf5586bc7")]
public partial class BlastBreathAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> charge;
    [SerializeReference] public BlackboardVariable<GameObject> blastBreath;
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;
    GameObject chargeInstance;
    GameObject blastBreathInstance;
    CapsuleCollider collider;
    float chargeDuration;
    float blastBreathDuration;
    float colliderDuration = 0.15f; // Duration of the prefabs z axis elongation from the vfx graph
    float blastBreathScale = 45f; // Scale from the vfx, size is ~49 but we want it to be larger so it goes past the target
    float distanceToTarget;
    float timer = 0f;
    bool isCharging = true;

    protected override Status OnStart()
    {
        // Instantiate vfx prefabs at the agent's position
        chargeInstance = GameObject.Instantiate(charge.Value, agent.Value.transform);

        // Get the charge and blast breath durations
        chargeDuration = chargeInstance.GetComponent<VisualEffect>().GetFloat("Lifetime");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if the charge animation is still playing
        if (isCharging)
        {
            // Look at the target while charging
            agent.Value.transform.LookAt(target.Value.transform);

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

            // Set the position, scale and orientation of the blast breath instance towards the target
            blastBreathInstance.transform.LookAt(target.Value.transform);
            blastBreathInstance.transform.localScale = new Vector3(1f, 1f, distanceToTarget / blastBreathScale);

            // Reset the timer for the blast breath
            timer = 0f;
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
            collider.height = currentSize / (distanceToTarget / blastBreathScale) * 0.9f; //Bypass the colliders height scale adjustment
            collider.center = new Vector3(0f, 0f, collider.height / 2f);
        }
    }
}

