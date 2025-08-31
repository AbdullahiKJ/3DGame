using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Combat : MonoBehaviour
{
    Movement movement;
    Animator animator;
    bool isPunching;
    float comboTimer = 0;
    public int comboLevel = 0;
    public int newComboLevel = 0;
    [SerializeField] float timeBetweenPunches = 1.5f;
    float timeBeforePunch = 0.75f;
    [SerializeField] List<GameObject> playerAttackColliders;
    public List<AttackSO> playerAttackSO;
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float enemyRange = 0.5f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask terrainLayer;
    public float playerHeight = 5f;
    List<GameObject> hitEnemies = new List<GameObject>();
    [SerializeField] DamageSO damageSO;
    [SerializeField] GameObject manager;
    TerrainEffects terrainEffects;
    bool colliderActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        terrainEffects = manager.GetComponent<TerrainEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        // Combo check
        if (isPunching)
        {
            movement.setMoveInput(Vector2.zero);

            comboTimer += Time.deltaTime / Time.timeScale;
            if (comboTimer > timeBetweenPunches)
            {
                comboTimer = 0;
                comboLevel = 0;
                isPunching = false;
                animator.SetBool("isPunching", false);
            }
            else if (comboLevel > newComboLevel)
            {
                newComboLevel = comboLevel;
                if (newComboLevel == 2)
                {
                    newComboLevel = -1;
                }
            }

            // get list of enemies/terrain hit and trigger animation and damage dealt
            Collider[] hitColliders = new Collider[5];
            Collider[] terrainColliders = new Collider[5];
            if (comboLevel == 0 && colliderActive)
            {
                Physics.OverlapSphereNonAlloc(playerAttackColliders[0].transform.position, attackRadius, hitColliders, enemyLayer);
                Physics.OverlapSphereNonAlloc(playerAttackColliders[0].transform.position, attackRadius, terrainColliders, terrainLayer);
            }
            else if (colliderActive)
            {
                Physics.OverlapSphereNonAlloc(playerAttackColliders[comboLevel - 1].transform.position, attackRadius, hitColliders, enemyLayer);
                Physics.OverlapSphereNonAlloc(playerAttackColliders[comboLevel - 1].transform.position, attackRadius, terrainColliders, terrainLayer);
            }

            foreach (Collider hit in hitColliders)
            {
                if (hit == null)
                    continue;

                GameObject newEnemy = hit.transform.root.gameObject;
                if (hitEnemies.Contains(newEnemy))
                {
                    // pass
                }
                else
                {
                    hitEnemies.Add(newEnemy);
                    Vector3 sphereCentre = playerAttackColliders[comboLevel == 0 ? 0 : comboLevel - 1].transform.position;
                    Vector3 contactPoint = hit.ClosestPoint(sphereCentre);
                    DamageManager damageManager = newEnemy.GetComponent<DamageManager>();
                    if (damageManager != null)
                    {
                        damageManager.TakeDamage(gameObject.transform.position, contactPoint, damageSO, enemyRange);
                    }
                }
            }

            foreach (Collider hit in terrainColliders)
            {
                if (hit == null)
                    continue;

                GameObject hitObject = hit.transform.root.gameObject;
                if (hitEnemies.Contains(hitObject))
                {
                    // pass
                }
                else
                {
                    hitEnemies.Add(hitObject);
                    Vector3 sphereCentre = playerAttackColliders[comboLevel == 0 ? 0 : comboLevel - 1].transform.position;
                    Vector3 contactPoint = hit.ClosestPoint(sphereCentre);

                    // Handle Terrain Impacts
                    terrainEffects.TerrainImpact(transform.position, hitObject, contactPoint, damageSO);
                }
            }
        }

        else
        {
            // reset the hit enemies list
            hitEnemies = new List<GameObject>();
        }
    }

    public bool getIsPunching()
    {
        return isPunching;
    }

    // trigger punch
    void OnPunch(InputValue value)
    {
        // Condition prevents triggering attacks before most of the animation has played out
        if (value.isPressed && (comboTimer > timeBeforePunch || comboTimer == 0) && !movement.isJumping)
        {
            // reset the hit enemies list
            hitEnemies = new List<GameObject>();

            // reset the collider actibe bool
            colliderActive = false;

            movement.turnOffSprint();
            movement.setMoveInput(Vector2.zero);
            isPunching = true;
            animator.SetBool("isPunching", true);
            comboTimer = 0;

            if (comboLevel < 3)
            {
                comboLevel++;
            }
            else if (comboLevel == 3)
            {
                comboLevel = 1;
            }

            animator.SetInteger("Combo", comboLevel);
            StartCoroutine(WaitForAnimationStateChange());
        }
    }

    void OnDrawGizmosSelected()
    {
        if (comboLevel != 0)
        {
            Gizmos.DrawSphere(playerAttackColliders[comboLevel - 1].transform.position, attackRadius);
        }
        else
        {
            Gizmos.DrawSphere(playerAttackColliders[0].transform.position, attackRadius);
        }

        Gizmos.DrawSphere(this.transform.position + new Vector3(0f, playerHeight, 0f), enemyRange);
    }

    public void playAttackVFX(int hitBoxIndex)
    {

        float vfxScale = Random.Range(0.5f, 0.8f);
        Vector3 vfxRotation = new Vector3(0f, 0f, Random.Range(-10f, 10f)) + playerAttackSO[hitBoxIndex].orientation;
        GameObject slashInstance = Instantiate(vfxPrefab);

        // Set the prefab rotation and scale
        slashInstance.transform.position = playerAttackColliders[hitBoxIndex].transform.position;
        slashInstance.transform.forward = transform.forward;
        slashInstance.transform.Rotate(vfxRotation);
        slashInstance.transform.localScale = Vector3.one * vfxScale;

        // Play the vfx
        slashInstance.GetComponent<VisualEffect>().Play();

        // Play the swing sound effect
        SoundFXManager.instance.PlaySoundFXClip(playerAttackSO[hitBoxIndex].soundFX, transform, 1f);
    }

    public void destroyAttackVFX()
    {
        GameObject[] slashPrefabs = GameObject.FindGameObjectsWithTag("SlashVfx");
        foreach (GameObject slashPref in slashPrefabs)
        {
            Destroy(slashPref);
        }
    }

    // Wait for the animation state to change - wait the transition duration
    IEnumerator WaitForAnimationStateChange()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        AnimatorStateInfo currentState = animator.GetNextAnimatorStateInfo(0);
        // Assign the new time variable
        timeBeforePunch = 0.6f * currentState.length;
    }
    void EnableCollider()
    {
        colliderActive = true;
    }
    void DisableCollider()
    {
        colliderActive = false;
    }
}
