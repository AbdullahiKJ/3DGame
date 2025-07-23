using UnityEngine;

public class SpecialCollision : MonoBehaviour
{
    [SerializeField] private DamageSO damageSO;
    private int playerLayerMask;

    void Awake()
    {
        playerLayerMask = LayerMask.NameToLayer("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayerMask)
        {
            // Get the enemy's damage manager component and apply damage
            DamageManager player = other.gameObject.GetComponent<DamageManager>();
            if (player != null)
            {
                // Deal damage
                player.TakeDamage(transform.position, default, damageSO);
            }
        }
    }
}
