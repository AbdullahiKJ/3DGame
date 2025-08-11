using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HitDetection", story: "Check [IsAttacking] variable, [AttackManager] AttackManager script, [enemyDamageSO] scriptable object and [Agent] for hits and update [State]", category: "Action", id: "828d7c607943a9112c00a96c3d306b8a")]
public partial class HitDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> IsAttacking;
    [SerializeReference] public BlackboardVariable<AttackManager> AttackManager;
    [SerializeReference] public BlackboardVariable<DamageSO> enemyDamageSO;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<State> State;
    GameObject weapon;
    float attackRadius = 1f;
    LayerMask playerLayer;
    LayerMask terrainLayer;
    GameObject manager;
    TerrainEffects terrainEffects;
    Vector3 previousContactPoint = Vector3.zero;
    float distanceThreshold = 0.1f;

    protected override Status OnStart()
    {
        AttackManager attackManager = Agent.Value.GetComponent<AttackManager>();
        weapon = attackManager.weapon;
        playerLayer = attackManager.detectionMask;
        terrainLayer = attackManager.terrainLayer;
        manager = attackManager.manager;
        terrainEffects = manager.GetComponent<TerrainEffects>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsAttacking && AttackManager.Value.canAttack)
        {
            // get list of enemies/terrain objects hit and trigger animation and damage dealt
            Collider[] hitColliders = new Collider[5];
            Collider[] terrainColliders = new Collider[5];
            Physics.OverlapSphereNonAlloc(weapon.transform.position, attackRadius, hitColliders, playerLayer);
            Physics.OverlapSphereNonAlloc(weapon.transform.position, attackRadius, terrainColliders, terrainLayer);


            foreach (Collider hit in hitColliders)
            {
                if (hit == null)
                    continue;

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
                    Vector3 contactPoint = hit.ClosestPoint(weapon.transform.position);
                    damageManager.TakeDamage(Agent.Value.gameObject.transform.position, contactPoint, enemyDamageSO.Value);
                }
            }

            foreach (Collider hit in terrainColliders)
            {
                GameObject hitObject = hit.transform.root.gameObject;
                Vector3 contactPoint = hit.ClosestPointOnBounds(weapon.transform.position);

                float distance = Vector3.Distance(contactPoint, previousContactPoint);

                // Handle Terrain Impacts
                if (distance > distanceThreshold)
                    terrainEffects.TerrainImpact(Agent.Value.transform.position, hitObject, contactPoint, 100f);

                previousContactPoint = contactPoint;
            }
        }
        return Status.Running;
    }
}

