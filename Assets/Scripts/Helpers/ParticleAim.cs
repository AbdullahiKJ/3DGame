using UnityEngine;

public class ParticleAim : MonoBehaviour
{
    public Transform target;
    float timer = 0f;
    float delay = 2f; // Aim delay
    Vector3 currentPosition;
    [SerializeField] DamageSO scytheDamageSO;
    int playerLayerMask;

    void Start()
    {
        currentPosition = transform.position;
        playerLayerMask = LayerMask.NameToLayer("Player");
    }

    void Update()
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

    void AimAtTarget()
    {
        transform.LookAt(currentPosition);

        // Remove the x and z rotation to keep the particle system upright
        Vector3 euler = transform.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        transform.eulerAngles = euler;

        // Update the current position to the target's position
        currentPosition = target.position;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.layer == playerLayerMask)
        {
            // Get the enemy's damage manager component and apply damage
            DamageManager player = other.gameObject.GetComponent<DamageManager>();
            if (player != null)
            {
                // Deal damage
                player.TakeDamage(transform.position, default, scytheDamageSO, 5f, true);
            }
        }
    }
}
