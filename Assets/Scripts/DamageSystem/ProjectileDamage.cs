using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] DamageSO damageSO;
    Vector3 player = Vector3.zero;
    public float maxFlyDistance = 100f;
    bool isMoving = false;
    int enemyLayer;
    int terrainLayer;
    [SerializeField] GameObject particlePrefab;
    GameObject manager;
    TerrainEffects terrainEffects;
    [SerializeField] AudioClip[] soundFX;
    [SerializeField] AudioClip[] hitSoundFX;

    void Awake()
    {
        manager = GameObject.Find("Manager");
        terrainEffects = manager.GetComponent<TerrainEffects>();
        // Play the sound FX
        SoundFXManager.instance.PlayRandomSoundFXClip(soundFX, transform, 0.5f);

        enemyLayer = LayerMask.NameToLayer("enemies");
        terrainLayer = LayerMask.NameToLayer("Terrain");
    }

    void Update()
    {
        // Move the projectile forward at the specified speed
        if (isMoving)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime * 1 / Time.timeScale, Space.World);

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
        ContactPoint contact = collision.GetContact(0);
        // Handle terrain impacts
        terrainEffects.TerrainImpact(transform.position, collision.gameObject, contact.point, 1.5f);

        // Play the sound FX
        SoundFXManager.instance.PlayRandomSoundFXClip(hitSoundFX, transform, 1f);

        if (collision.gameObject.layer == enemyLayer)
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
        else if (collision.gameObject.layer == terrainLayer)
        {
            Instantiate(particlePrefab, contact.point, Quaternion.identity);

            // Destroy the projectile on collision with the environment
            Destroy(gameObject);
        }
    }
}
