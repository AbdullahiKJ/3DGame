using UnityEngine;

public class SpecialCollision : MonoBehaviour
{
    [SerializeField] private DamageSO damageSO;
    private int playerLayerMask;
    GameObject manager;
    TerrainEffects terrainEffects;

    void Awake()
    {
        playerLayerMask = LayerMask.NameToLayer("Player");
        manager = GameObject.Find("Manager");
        terrainEffects = manager.GetComponent<TerrainEffects>();
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 contact = other.ClosestPointOnBounds(transform.position);
        // Handle terrain impacts
        terrainEffects.TerrainImpact(transform.position, other.gameObject, contact, 200f);

        if (other.gameObject.layer == playerLayerMask)
        {
            // Get the enemy's damage manager component and apply damage
            DamageManager player = other.gameObject.GetComponent<DamageManager>();
            if (player != null)
            {
                // todo: get hit sound effect for the blast breath
                // Deal damage
                player.TakeDamage(transform.position, default, damageSO, 7.5f, true);
            }
        }
    }
}
