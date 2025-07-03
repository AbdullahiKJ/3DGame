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
    public float playerHeight = 5f;
    List<GameObject> hitEnemies = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        // Combo check
        if(isPunching)
        {
            movement.setMoveInput(Vector2.zero);

            comboTimer += Time.deltaTime;
            if (comboTimer > timeBetweenPunches)
            {
                comboTimer = 0;
                comboLevel = 0;
                isPunching = false;
                animator.SetBool("isPunching", false);
            }
            else if(comboLevel > newComboLevel)
            {
                newComboLevel = comboLevel;
                if  (newComboLevel == 2)
                {
                    newComboLevel = -1;
                }
            }

            // get list of enemies hit and trigger animation and damage dealt
            Collider[] hitColliders;
            if(comboLevel == 0)
            {
                hitColliders = Physics.OverlapSphere(playerAttackColliders[0].transform.position, attackRadius, enemyLayer);
            }
            else
            {
                hitColliders = Physics.OverlapSphere(playerAttackColliders[comboLevel-1].transform.position, attackRadius, enemyLayer);
            }

            
            // Debug.Log(hitColliders.Length);
            foreach(Collider hit in hitColliders)
            {
                GameObject newEnemy = hit.transform.root.gameObject;
                if (hitEnemies.Contains(newEnemy))
                {
                    // pass
                }
                else
                {
                    hitEnemies.Add(newEnemy);
                    newEnemy.GetComponent<DamageManager>().TakeDamage(gameObject.transform.position, enemyRange);
                }
                
            }
        }

        else
        {
            // reset the hit enemies list
            hitEnemies = new List<GameObject>();
        }
    }

    public bool getIsPunching() {
        return isPunching;
    }

    // trigger punch
    void OnPunch(InputValue value)
    {
        // Condition prevents tirggering attacks before most of the animation has played out
        if(value.isPressed && (comboTimer > timeBeforePunch || comboTimer == 0))
        {
            // reset the hit enemies list
            hitEnemies = new List<GameObject>();

            movement.turnOffSprint();
            movement.setMoveInput(Vector2.zero);
            isPunching = true;
            animator.SetBool("isPunching", true);
            comboTimer = 0;

            if(comboLevel < 3)
            {
                comboLevel ++;
            }
            else if(comboLevel == 3)
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
            Gizmos.DrawSphere(playerAttackColliders[comboLevel-1].transform.position, attackRadius);
        }
        else
        {
            Gizmos.DrawSphere(playerAttackColliders[0].transform.position, attackRadius);
        }

        Gizmos.DrawSphere(this.transform.position + new Vector3(0f, playerHeight, 0f), enemyRange);
    }

    public void playAttackVFX(int hitBoxIndex) {

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
    }

    public void destroyAttackVFX() {
        GameObject[] slashPrefabs = GameObject.FindGameObjectsWithTag("SlashVfx");
        foreach (GameObject slashPref in slashPrefabs) {
            Destroy(slashPref);
        }
     }

    // Wait for the animation state to change - wait the transition duration
    IEnumerator WaitForAnimationStateChange()
    {
        yield return new WaitForSeconds(0.1f);
        AnimatorStateInfo currentState = animator.GetNextAnimatorStateInfo(0);
        // Assign the new time variable
        timeBeforePunch = 0.6f * currentState.length;
    }
}
