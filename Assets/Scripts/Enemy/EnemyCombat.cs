using UnityEngine;
using System.Collections;

public class EnemyCombat : MonoBehaviour
{
    Animator animator;
    AI aI;
    CharacterController controller;
    [SerializeField] float moveControllerSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        aI = GetComponent<AI>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(moveControllerSpeed != 0f) {
            controller.Move(transform.forward * moveControllerSpeed);
        }
    }

    public void triggerAttack(int attackLevel) {
        // choose the attack animation
        switch (attackLevel) {
            case 1: 
                triggerComboOne();
                break;
            case 2:
                triggerComboTwo();
                break;
        }
    }

    public void triggerComboOne()
    {
        animator.SetInteger("attackLevel", 1);
        StartCoroutine(WaitForAnimation(4.2f));
    }

    void triggerComboTwo()
    {
        animator.SetInteger("attackLevel", 2);
        StartCoroutine(WaitForAnimation(4.667f));
    }

    public void cancelAttack()
    {
        animator.SetInteger("attackLevel", 0);
        animator.SetBool("isAttacking", false);
    }

    IEnumerator WaitForAnimation(float animationTime)
    {
        // start the attack animation
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(animationTime);
        // change back to idle state after animation ends
        animator.SetBool("isAttacking", false);
        moveControllerSpeed = 0f;
        // reset the do not interrupt variable
        aI.setDoNotInterrupt();
    }
}
