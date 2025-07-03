using System.Collections;
using Unity.UI.Shaders.Sample;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    Animator animator;
    CharacterController controller;
    [SerializeField] float maxHealth = 100f;
    float currentHealth;
    [SerializeField] float stagger = 100f;
    public bool isStaggering { get; set; } = false;
    [SerializeField] RangeBar healthBar;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;
    }

    // TODO: pass attack data to use
    public void TakeDamage(Vector3 attacker, float enemyRange = 0f)
    {
        // If the character is already staggering, do not take damage again
        if (isStaggering)
        {
            return;
        }
        else
        {
            isStaggering = true;
        }

        // Reduce health and stagger 
        currentHealth -= 10f; // Example damage value
        healthBar.Value = new Vector2(1 - currentHealth / maxHealth, healthBar.Value.y);

        // face the attacker and move the game object to an appropriate distance from the attacker
        FaceAttacker(attacker, enemyRange);

        animator.SetTrigger("isHit");
        animator.SetFloat("Speed", 69f);

        // Character cannot take damage for half of the stagger animation time
        float staggerLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(staggerLength);
        StartCoroutine(WaitForStaggerAnimation(staggerLength));
    }

    // make the gameobject face the game object dealing damage
    public void FaceAttacker(Vector3 attacker, float enemyRange)
    {
        transform.LookAt(attacker, Vector3.up);

        // move the damage receiver to an appropriate distance
        Vector2 posDifference = new Vector2(attacker.x - transform.position.x, attacker.y - transform.position.y);
        float posDiffMagnitude = posDifference.magnitude;
        // controller.Move(transform.forward * -1 * (enemyRange - posDiffMagnitude));
        controller.Move(transform.forward * -1 * posDiffMagnitude * 0.1f);
    }

    // TODO: store stats for each attack and calculate damage based on those stats
    void CalculateDamage()
    {

    }

    IEnumerator WaitForStaggerAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        isStaggering = false;
        animator.ResetTrigger("isHit");
    }
}
