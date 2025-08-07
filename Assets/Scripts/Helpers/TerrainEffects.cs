using UnityEngine;

public class TerrainEffects : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] GameObject dirtParticlePrefab;

    public void TerrainImpact(Vector3 attackPos, GameObject hitObject, Vector3 contactPoint)
    {
        if (hitObject.layer == terrainLayer)
        {
            // Trigger explosion if the object is destructible
            if (hitObject.CompareTag("Destructible"))
            {
                DestroyTerrain destroy = hitObject.GetComponent<DestroyTerrain>();
                if (destroy != null)
                    destroy.TriggerExplosion(attackPos);
            }
            // Otherwise play a dirt particle system
            else
            {
                Instantiate(dirtParticlePrefab, contactPoint, Quaternion.identity);
            }
        }
    }
}
