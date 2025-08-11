using UnityEngine;

public class Detector : MonoBehaviour
{
    float attackDistance = 2.5f;
    [SerializeField] float minMidRangeDistance = 9.5f;
    [SerializeField] float maxMidRangeDistance = 20f;
    public LayerMask detectionMask;
    [SerializeField] GameObject target;

    // Check if the player is within the base attack range
    public bool CheckBaseAttackRange()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, attackDistance, new Collider[1], detectionMask);
        return hits > 0;
    }

    public float CheckWithinMidRange()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > minMidRangeDistance && distance < maxMidRangeDistance)
        {
            float animatorFactor = (distance - minMidRangeDistance) / (maxMidRangeDistance - minMidRangeDistance);
            return animatorFactor;
        }
        else
            return -1f;
    }

    public bool CheckWithinLongRange()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > maxMidRangeDistance)
            return true;
        else
            return false;
    }
}
