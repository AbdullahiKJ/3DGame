using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class TrackingLightning : MonoBehaviour
{
    [SerializeField] GameObject singleLightningPrefab;
    [SerializeField] GameObject indicatorPrefab;
    GameObject indicatorInstance;
    ParticleSystem indicatorParticleSystem;
    GameObject lightningInstance;
    public Transform targetTransform;
    float indicatorDuration = 5f;
    float intervalDuration = 1f; // Duration between lightning strikes
    float strikeDuration;
    bool isTracking = true;
    [SerializeField] DamageSO damageSO;
    float aoeRadius = 2f; // Area of effect radius for damage
    int playerLayerBitMask;
    [SerializeField] GameObject explosionPrefab;
    GameObject explosionInstance;
    ParticleSystem explosionParticleSystem;

    void Start()
    {
        // Get the player layer bitmask
        playerLayerBitMask = LayerMask.GetMask("Player");

        // Instantiate the explosion prefab
        explosionInstance = Instantiate(explosionPrefab, Vector3.zero, Quaternion.identity);
        explosionParticleSystem = explosionInstance.GetComponent<ParticleSystem>();

        // Briefly instantiate the lightning strike to get the duration
        lightningInstance = Instantiate(singleLightningPrefab, Vector3.zero, Quaternion.identity);
        strikeDuration = lightningInstance.GetComponent<VisualEffect>().GetFloat("Lifetime");
        Destroy(lightningInstance); // Destroy the brief instance

        Vector3 initialPosition = GetTargetPosition();
        indicatorInstance = Instantiate(indicatorPrefab, initialPosition, Quaternion.identity);

        // Get the ParticleSystem component from the indicator instance
        indicatorParticleSystem = indicatorInstance.GetComponent<ParticleSystem>();

        // Set the particle system properties for the indicator
        var main = indicatorParticleSystem.main;
        main.duration = indicatorDuration;
        main.startLifetime = indicatorDuration;

        // Play the indicator particle system
        indicatorParticleSystem.Play();
        StartCoroutine(WaitForTrackingEnd());
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the target position if tracking is enabled
        if (isTracking)
        {
            Vector3 newPosition = GetTargetPosition();
            indicatorInstance.transform.position = newPosition;
        }
    }

    IEnumerator WaitForTrackingEnd()
    {
        // Wait for 80% of the indicator duration before stopping tracking
        yield return new WaitForSeconds(indicatorDuration * 0.8f);
        isTracking = false;
        // Wait for the remaining 20% and trigger the lightning strike
        yield return new WaitForSeconds(indicatorDuration * 0.2f);
        lightningInstance = Instantiate(singleLightningPrefab, indicatorInstance.transform.position, Quaternion.identity);
        StartCoroutine(WaitForStrikeEnd());
    }

    IEnumerator WaitForStrikeEnd()
    {
        // Play explosion effect
        explosionInstance.transform.position = indicatorInstance.transform.position;
        explosionParticleSystem.Play();
        DealDamage();

        // Wait for the lightning strike to finish
        yield return new WaitForSeconds(strikeDuration);
        Destroy(lightningInstance);

        // Wait the interval duration before allowing the next strike
        yield return new WaitForSeconds(intervalDuration);
        isTracking = true;
        indicatorParticleSystem.Play();
        StartCoroutine(WaitForTrackingEnd());
    }

    void DealDamage()
    {
        // Check if the player is in the area of effect and deal damage
        Collider[] hitColliders = Physics.OverlapSphere(indicatorInstance.transform.position, aoeRadius, playerLayerBitMask);
        if (hitColliders.Length > 0)
        {
            hitColliders[0].gameObject.GetComponent<DamageManager>().TakeDamage(indicatorInstance.transform.position, default, damageSO);
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 initialPosition = targetTransform.position;
        initialPosition.y = 0.1f; // Ensure the lightning starts just above ground level
        return initialPosition;
    }

    public void DestroyInstances()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
        }
        if (lightningInstance != null)
        {
            Destroy(lightningInstance);
        }
        if (explosionInstance != null)
        {
            Destroy(explosionInstance);
        }
        StopAllCoroutines(); // Stop any ongoing coroutines
    }
}
