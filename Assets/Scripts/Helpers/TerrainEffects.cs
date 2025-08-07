using UnityEngine;

public class TerrainEffects : MonoBehaviour
{
    [SerializeField] GameObject dirtParticlePrefab;
    int terrainLayer;

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
            // Otherwise play a dirt particle system
            else if (ignoreParticle)
            {
                // continue
            }
            else
            {
                Instantiate(dirtParticlePrefab, contactPoint, Quaternion.identity);
            }
        }
    }
}
