using UnityEngine;

public class SpecialAttacks : MonoBehaviour
{
    [SerializeField] Animator specialAnimator;
    EnemyMovement enemyMovement;
    bool isAttacking = false;
    bool isMoving = false;
    float jumpTimer = 1.5f;
    float attackTimer = 5f;
    float currentTImer = 0f;
    [SerializeField] GameObject vfxContainer;
    [SerializeField] GameObject blastBreath;
    GameObject currentSpecial;
    [SerializeField] GameObject player;
    AI aiScript;
    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        aiScript = GetComponent<AI>();
    }

    void Update()
    {
        if (isMoving) {
            enemyMovement.jumpBack();
            currentTImer += Time.deltaTime;
            if (currentTImer > jumpTimer) {
                isMoving = false;
                enemyMovement.cancelMovement();
                triggerAttack();
            }
        }

        if (isAttacking) {
            currentTImer += Time.deltaTime;
            if (currentTImer > attackTimer) {
                isAttacking = false;
                currentTImer = 0f;
                // Destroy prefab
                Destroy(currentSpecial);
                aiScript.setDoNotInterrupt();
            }
            return;
        }
    }

    void triggerAttack() {
        currentSpecial = Instantiate(blastBreath, vfxContainer.transform);
        currentSpecial.transform.LookAt(player.transform.position);
        isAttacking = true;
        currentTImer = 0f;
    }

    public bool getIsAttacking() {
        return isAttacking;
    }

    public void triggerSpecial() {
        isMoving = true;
    }
}
