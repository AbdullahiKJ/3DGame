using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SideStep", story: "[Agent] side steps while facing [Target]", category: "Action", id: "b49aa6ad21d4a565b76197675835634d")]
public partial class SideStepAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    NavMeshAgent navMeshAgent;
    Vector3 sideStepTarget;
    Animator animator;
    int stepDirection;
    float sideStepTimer;
    float timer;
    Vector3 targetLookAt = Vector3.zero;
    float rotationAdjustment = 110f;

    // TODO: Easing variables
    bool isEasing = true;
    bool isEasingOut = true;
    float easingDuration = 0.5f;
    float animationFactor = 0f;

    protected override Status OnStart()
    {
        animator = Agent.Value.GetComponent<Animator>();
        navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();

        // Face the target while side stepping
        targetLookAt = new Vector3(Target.Value.transform.position.x, Agent.Value.transform.position.y, Target.Value.transform.position.z);
        Agent.Value.transform.LookAt(targetLookAt);

        // Determine side step target
        stepDirection = UnityEngine.Random.Range(0, 2) * 2 - 1;

        // Set the animation parameters
        animator.SetFloat("HorizontalInput", 1f * stepDirection);

        // Set a random timer for the side step
        sideStepTimer = UnityEngine.Random.Range(2f, 7f);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // TODO: Ease the agents movement when the side step starts and comes to an end
        // if (isEasing)
        // {
        //     animationFactor += Time.deltaTime / easingDuration;
        //     if (animationFactor >= 1f)
        //     {
        //         animationFactor = 1f;
        //         isEasing = false;
        //         isEasingOut = false;
        //     }
        //     animator.SetFloat("HorizontalInput", animationFactor);
        // }
        // else if (isEasingOut)
        // {
        //     animationFactor -= Time.deltaTime / easingDuration;
        //     if (animationFactor <= 0f)
        //     {
        //         animationFactor = 0f;
        //         isEasingOut = false;
        //     }
        //     animator.SetFloat("HorizontalInput", Mathf.Lerp(1f * stepDirection, 0f, animationFactor));
        // }

        if (timer > sideStepTimer)
        {
            return ResetAgent();
        }
        timer += Time.deltaTime;

        Vector3 playerOffset = Agent.Value.transform.position - Target.Value.transform.position;

        // adjust the value of the rotation speed based on the distance from the player
        float circleRotationSpeed;
        circleRotationSpeed = 1 / playerOffset.magnitude * rotationAdjustment * stepDirection * -1f;

        // get the destination for the next frame to move in a circle
        Vector3 destination = Quaternion.Euler(0, circleRotationSpeed, 0) * playerOffset + Target.Value.transform.position;
        Vector3 agentVelocity = destination - Agent.Value.transform.position;

        // If the agent cannot reach the destination, exit the action
        if (navMeshAgent.Raycast(destination, out NavMeshHit hit))
        {
            return ResetAgent();
        }

        // Face the target while side stepping
        targetLookAt = new Vector3(Target.Value.transform.position.x, Agent.Value.transform.position.y, Target.Value.transform.position.z);
        Agent.Value.transform.LookAt(targetLookAt);

        navMeshAgent.Move(agentVelocity * Time.deltaTime);

        return Status.Running;
    }

    Status ResetAgent()
    {
        // Side step is complete
        timer = 0f;
        animator.SetFloat("HorizontalInput", 0f);
        return Status.Success;
    }
}

