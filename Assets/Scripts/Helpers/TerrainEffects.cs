using UnityEngine;

public class TerrainEffects : MonoBehaviour
{
    [SerializeField] GameObject dirtParticlePrefab;
    int terrainLayer;
    [SerializeField] AudioClip[] defaultSoundFX;

    void Start()
    {
        terrainLayer = LayerMask.NameToLayer("Terrain");
    }

    public void TerrainImpact(Vector3 attackPos, GameObject hitObject, Vector3 contactPoint, float forceMultiplier = 1f, bool ignoreParticle = false)
    {
        if (hitObject.layer == terrainLayer)
        {
            // Trigger explosion if the object is destructible
            if (hitObject.CompareTag("Destructible"))
            {
                DestroyTerrain destroy = hitObject.GetComponent<DestroyTerrain>();
                if (destroy != null)
                    destroy.TriggerExplosion(attackPos, forceMultiplier);
            }
            else if (ignoreParticle)
            {
                // Don't play any partcile systems
            }
            // Otherwise play a dirt particle system
            else
            {
                Instantiate(dirtParticlePrefab, contactPoint, Quaternion.identity);
                // Play sound effects
                SoundFXManager.instance.PlayRandomSoundFXClip(defaultSoundFX, hitObject.transform, 1f);
            }
        }
    }
}
