using System.Collections;
using UnityEngine;

public class AI : MonoBehaviour
{
    EnemyMovement enemyMovement;
    EnemyCombat enemyCombat;
    SpecialAttacks specialAttacks;
    CharacterController controller;
    [SerializeField] GameObject player;
    [SerializeField] float attackDistance = 5f;

    [SerializeField] float thinkingDistance = 10f;

    [SerializeField] float approachDistance = 30f;
    float timeBetweenIdle;
    public float timeBetweenAttacks;
    public float attackTimer;
    public float idleTimer;
    public enum State {
        Idle,
        Approaching,
        Attacking,
        Special
    };
    public State currentState = State.Idle;
    State previousState = State.Idle;
    public bool doNotInterrupt = false;
    bool moveDirection;
    public bool walkBack = false;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyCombat = GetComponent<EnemyCombat>();
        specialAttacks = GetComponent<SpecialAttacks>();
        controller = GetComponent<CharacterController>();

        timeBetweenIdle = Random.Range(6f, 10f);
        timeBetweenAttacks = Random.Range(4f, 6f);

        attackTimer = timeBetweenAttacks;
        idleTimer = timeBetweenIdle;

        // Test special attack
        // specialAttacks.triggerSpecial();
    }

    void Update()
    {
        // Test special attack
        // doNotInterrupt = true;

        // if an action that should not be interrupted has started, ignore the update method
        if (doNotInterrupt) {
            return;
        }

        float playerDistance = (player.transform.position - transform.position).magnitude;

        // increase the timer if it is less than the set period
        if (attackTimer < timeBetweenAttacks) {
            attackTimer += Time.deltaTime;
        }

        // Trigger special attack
        // if (playerDistance > approachDistance) {
        //     currentState = State.Special;
        //     doNotInterrupt = true;
        //     specialAttacks.triggerSpecial();
        // }

    
        // Approach if within approach distance
        else if (thinkingDistance  <= playerDistance && playerDistance <= approachDistance) {
            currentState = State.Approaching;
        }

        // Decide actions when in the thinking range
        else if (attackDistance <= playerDistance  && playerDistance <= thinkingDistance) {
            currentState = State.Idle;
        }

        // Attack player if in range and not in cooldown period
        else if (playerDistance <= attackDistance) {
            if (attackTimer >= timeBetweenAttacks) {
                currentState = State.Attacking;
            }
            if (currentState == State.Idle) {
                walkBack = true;
            }
        }

        CancelActions(false);   

        // Perform an action based on the current state
        switch (currentState) 
        {
            case State.Idle:
                RunIdle();
                break;
            case State.Approaching:
                RunApproach();
                break;
            case State.Attacking:
                RunAttack();
                break;
        }
    }

    void RunIdle()
    {
        // After a period of time, trigger a new idle action
        if (idleTimer >= timeBetweenIdle)
        {
            CancelActions(true);
            idleTimer = 0f;

            // move back if within range
            if (walkBack) {
                enemyMovement.setIsWalking(true, false);
                return;
            }
            
            // choose an idle action: regular approach, circle, attack, etc.
            int action = Mathf.FloorToInt(Random.Range(1, 4.9f));

            switch (action)
            {
                case 1:
                    enemyMovement.setIsCircling(true, moveDirection);
                    break;
                case 2:
                    enemyMovement.setIsWalking(true, moveDirection);
                    break;
                case 3:
                    enemyMovement.setIsWalking(true, true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (walkBack) {
                walkBack = false;
            }
        }
    }

    void RunApproach()
    {
        // approach player
        enemyMovement.setIsWalking(true, true);
    }

    void RunAttack()
    {
        CancelActions(true);

        // choose an attack
        int attack = Mathf.FloorToInt(Random.Range(1, 2.9f));
        enemyCombat.triggerAttack(attack);
        doNotInterrupt = true;
    }

    void CancelActions(bool forceCancel)
    {
        if (previousState != currentState || forceCancel)
        {
            timeBetweenIdle = Random.Range(6f, 10f);
            timeBetweenAttacks = Random.Range(4f, 6f);

            // cancel actions
            enemyMovement.cancelMovement();
            enemyCombat.cancelAttack();

            // reset timers and state
            previousState = currentState;
            idleTimer = timeBetweenIdle;

            // choose a random move direction for approach and circle
            moveDirection = Mathf.FloorToInt(Random.Range(0, 1.9f)) == 0;
            StartCoroutine(WaitForAnimation());
        }
    }

    public void setDoNotInterrupt() {
        doNotInterrupt = false;
        attackTimer = 0;
        currentState = State.Idle;
    }

    // Draw attack, thinking and approach distances
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, thinkingDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, approachDistance);

    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f);
    }
}
