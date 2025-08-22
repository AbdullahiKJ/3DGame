using UnityEngine;

public class RecordExplosion : MonoBehaviour
{
    public float minForce = 100f;
    public float maxForce = 400f;

    void Start()
    {
        foreach (Transform t in transform)
        {
            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = transform.forward * -1;
                Vector3 force = direction * Random.Range(minForce, maxForce);
                rb.AddForce(force);
            }
        }
    }
}