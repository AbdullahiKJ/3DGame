using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteors : MonoBehaviour
{
    [SerializeField] float duration = 60f;
    [SerializeField] DamageSO damageSO;

    [SerializeField] LayerMask playerLayerIndex;
    [SerializeField] GameObject lavaPrefab;
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    ParticleSystem meteorParticleSystem;
    GameObject terrain;
    public MeteorAction meteorAction;
    GameObject manager;
    TerrainEffects terrainEffects;
    [SerializeField] AudioClip ambientSound;
    [SerializeField] AudioClip hitSoundFX;

    void Start()
    {
        meteorParticleSystem = GetComponent<ParticleSystem>();
        terrain = GameObject.Find("Terrain");

        manager = GameObject.Find("Manager");
        terrainEffects = manager.GetComponent<TerrainEffects>();
        StartCoroutine(WaitForDuration());

        // Play the sound FX
        SoundFXManager.instance.PlayAmbientClip(ambientSound, transform, 1f, duration);
    }

    void OnParticleCollision(GameObject other)
    {
        // Handle terrain impacts
        terrainEffects.TerrainImpact(transform.position, other, other.transform.position, damageSO, 150f, true);

        if (other.gameObject.layer == playerLayerIndex)
        {
            DealDamage(other);
        }
        else
        {
            meteorParticleSystem.GetCollisionEvents(terrain, collisionEvents);
            foreach (ParticleCollisionEvent collision in collisionEvents)
            {
                // Create a lava prefab and instantiate it at the collision point
                float randomRotation = Random.Range(0f, 360f);
                Quaternion lavaRotation = Quaternion.Euler(-90f, randomRotation, 0f);

                Vector3 spawnPosition = collision.intersection + new Vector3(0f, 0.01f, 0f); // Slightly above the terrain to avoid clipping
                Instantiate(lavaPrefab, spawnPosition, lavaRotation);
            }
        }

        // Play the sound FX every other collision
        int rand = Random.Range(0, 3);
        if (rand == 0)
            SoundFXManager.instance.PlaySoundFXClip(hitSoundFX, transform, 0.5f, 0.8f, 1.2f);
    }

    IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        // Reset the meteor action flag
        if (meteorAction != null)
        {
            meteorAction.meteorFlag.Value = false; // Set the meteor flag to false
        }

        // Destroy the meteor rain after the duration
        Destroy(gameObject);
    }

    void DealDamage(GameObject other)
    {
        // Get the enemy's damage manager component and apply damage
        DamageManager player = other.gameObject.GetComponentInParent<DamageManager>();
        if (player != null)
        {
            // Deal damage
            player.TakeDamage(transform.position, default, damageSO);
        }
    }
}
