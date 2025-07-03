using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HitDetection", story: "Check [IsAttacking] variable, [AttackManager] AttackManager script and [Agent] for hits and update [State]", category: "Action", id: "828d7c607943a9112c00a96c3d306b8a")]
public partial class HitDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> IsAttacking;
    [SerializeReference] public BlackboardVariable<AttackManager> AttackManager;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<State> State;
    GameObject weapon;
    float attackRadius = 1f;
    LayerMask playerLayer;

    protected override Status OnStart()
    {
        weapon = Agent.Value.GetComponent<AttackManager>().weapon;
        playerLayer = Agent.Value.GetComponent<AttackManager>().detectionMask;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsAttacking && AttackManager.Value.canAttack)
        {
            // get list of enemies hit and trigger animation and damage dealt
            Collider[] hitColliders;
            hitColliders = Physics.OverlapSphere(weapon.transform.position, attackRadius, playerLayer);

            foreach (Collider hit in hitColliders)
            {
                GameObject newEnemy = hit.transform.gameObject;
                DamageManager damageManager = newEnemy.GetComponent<DamageManager>();
                // if (hitEnemies.Contains(newEnemy))
                // {
                // pass
                // }
                if (damageManager.isStaggering)
                {
                    //pass
                }
                else
                {
                    // hitEnemies.Add(newEnemy);
                    damageManager.TakeDamage(Agent.Value.gameObject.transform.position);
                }
            }
        }
        return Status.Running;
    }
}

