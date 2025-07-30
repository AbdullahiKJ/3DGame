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

    void Start()
    {
        currentPosition = target.transform.position;
        playerLayerIndex = LayerMask.NameToLayer("PlayerParticle");
        playerLayerBitMask = LayerMask.GetMask("PlayerParticle");
        scytheParticleSystem = GetComponent<ParticleSystem>();
        terrain = GameObject.Find("Terrain");
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
        if (other.gameObject.layer == playerLayerIndex)
        {
            Debug.Log("direct collision");
            DealDamage(other);
        }
        // Apply AOE damage if the particle collides with the environment
        else
        {
            scytheParticleSystem.GetCollisionEvents(terrain, collisionEvents);
            foreach (ParticleCollisionEvent collision in collisionEvents)
            {
                // Get all colliders in the area of effect
                Collider[] hitColliders = Physics.OverlapSphere(collision.intersection, aoeRadius, playerLayerBitMask);
                foreach (var hitCollider in hitColliders)
                {
                    Debug.Log("aoe collision");
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
