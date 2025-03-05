using System.Collections;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(Vector3 attacker, float enemyRange)
    {
        // face the attacker and move the game object to an appropriate distance from the attacker
        FaceAttacker(attacker, enemyRange);

        WaitForStaggerAnimation(5f);
        animator.SetTrigger("isHit");
        // WaitForStaggerAnimation(0.967f);
        // animator.ResetTrigger("isHit");
    }

    // make the gameobject face the game object dealing damage
    public void FaceAttacker(Vector3 attacker, float enemyRange)
    {
        transform.LookAt(attacker, Vector3.up);

        // move the damage receiver to an appropriate distance
        Vector2 posDifference = new Vector2(attacker.x - transform.position.x, attacker.y - transform.position.y);
        float posDiffMagnitude = posDifference.magnitude;
        // controller.Move(transform.forward * -1 * (enemyRange - posDiffMagnitude));
    }

    IEnumerator WaitForStaggerAnimation(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
