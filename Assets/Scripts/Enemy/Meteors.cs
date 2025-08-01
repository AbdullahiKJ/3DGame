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

    void Start()
    {
        meteorParticleSystem = GetComponent<ParticleSystem>();
        terrain = GameObject.Find("Terrain");
        StartCoroutine(WaitForDuration());
    }

    void OnParticleCollision(GameObject other)
    {
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
                Instantiate(lavaPrefab, collision.intersection, lavaRotation);
            }
        }
    }

    IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        // Destroy the meteor rain after the duration
        Destroy(gameObject);

        GameObject[] lavaInstances = GameObject.FindGameObjectsWithTag("Lava");
        foreach (GameObject instance in lavaInstances)
        {
            Destroy(instance);
        }
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
