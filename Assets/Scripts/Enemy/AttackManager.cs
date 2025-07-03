using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public LayerMask detectionMask;
    public GameObject weapon;
    public bool canAttack = false;

    // Update is called once per frame
    void DisableAttack()
    {
        canAttack = false;
    }
    void EnableAttack()
    {
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(weapon.transform.position, 1);
    }
}
