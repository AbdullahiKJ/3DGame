using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] DamageSO damageSO;
    Vector3 player = Vector3.zero;
    public float maxFlyDistance = 100f;
    bool isMoving = false;
    int enemyLayerMask;
    int defaultLayerMask;
    [SerializeField] GameObject particlePrefab;

    void Awake()
    {
        enemyLayerMask = LayerMask.NameToLayer("enemies");
        defaultLayerMask = LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        // Move the projectile forward at the specified speed
        if (isMoving)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

            // Destroy the projectile after a certain distance (optional)
            if (Vector3.Distance(transform.position, player) > maxFlyDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Launch(Vector3 target, Vector3 playerPos)
    {
        player = playerPos;
        transform.LookAt(target);
        isMoving = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == enemyLayerMask)
        {
            // Get the enemy's damage manager component and apply damage
            DamageManager enemy = collision.gameObject.GetComponent<DamageManager>();
            if (enemy != null)
            {
                // Deal damage
                Vector3 contactPoint = collision.collider.ClosestPoint(transform.position);
                enemy.TakeDamage(transform.position, contactPoint, damageSO);
            }

            // Destroy the projectile after hitting an enemy
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == defaultLayerMask)
        {
            ContactPoint contact = collision.GetContact(0);
            Instantiate(particlePrefab, contact.point, Quaternion.identity);

            // Destroy the projectile on collision with the environment
            Destroy(gameObject);
        }
    }
}
