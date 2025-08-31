using System.Collections.Generic;
using UnityEngine;

public class ParticleAim : MonoBehaviour
{
    public Transform target;
    float timer = 0f;
    float delay = 2f; // Aim delay
    Vector3 currentPosition;
    [SerializeField] DamageSO scytheDamageSO;
    int playerLayerIndex;
    int playerLayerBitMask;
    float aoeRadius = 5f; // Area of effect radius for damage
    float rotationAdjustment = 15f; // Adjust the rotation to aim just above the target
    [SerializeField] bool isDirectional;
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    ParticleSystem scytheParticleSystem;
    GameObject terrain;
    GameObject manager;
    TerrainEffects terrainEffects;

    void Start()
    {
        currentPosition = target.transform.position;
        playerLayerIndex = LayerMask.NameToLayer("PlayerParticle");
        playerLayerBitMask = LayerMask.GetMask("PlayerParticle");
        scytheParticleSystem = GetComponent<ParticleSystem>();
        terrain = GameObject.Find("Terrain");
        manager = GameObject.Find("Manager");
        terrainEffects = manager.GetComponent<TerrainEffects>();
    }

    void Update()
    {
        if (isDirectional)
        {
            if (timer < delay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                AimAtTarget();
            }
        }
    }

    void AimAtTarget()
    {
        transform.LookAt(currentPosition);

        // Adjust the rotation to aim just above the target
        Vector3 euler = transform.eulerAngles;
        euler.x = rotationAdjustment;
        euler.z = 0f;
        transform.eulerAngles = euler;

        // Update the current position to the target's position
        currentPosition = target.position;
    }

    void OnParticleCollision(GameObject other)
    {
        // Handle terrain impacts
        terrainEffects.TerrainImpact(transform.position, other, other.transform.position, null, 100f, true);

        if (other.gameObject.layer == playerLayerIndex)
        {
            DealDamage(other);
        }
        // Apply AOE damage if the particle collides with the environment
        else
        {
            scytheParticleSystem.GetCollisionEvents(terrain, collisionEvents);
            foreach (ParticleCollisionEvent collision in collisionEvents)
            {
                // Get all colliders in the area of effect
                Collider[] hitColliders = new Collider[5];
                Physics.OverlapSphereNonAlloc(collision.intersection, aoeRadius, hitColliders, playerLayerBitMask);
                foreach (var hitCollider in hitColliders)
                {
                    DealDamage(hitCollider.gameObject);
                }
            }
        }
    }

    void DealDamage(GameObject other)
    {
        // Get the enemy's damage manager component and apply damage
        DamageManager player = other.gameObject.GetComponentInParent<DamageManager>();
        if (player != null)
        {
            // Deal damage
            player.TakeDamage(transform.position, default, scytheDamageSO);
        }
    }
}
