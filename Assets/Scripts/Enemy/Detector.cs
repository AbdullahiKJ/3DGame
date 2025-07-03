using UnityEngine;

public class Detector : MonoBehaviour
{
    float attackDistance = 2.5f;
    public LayerMask detectionMask;

    // Check if the player is within the base attack range
    public bool CheckBaseAttackRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackDistance, detectionMask);

        if (colliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
