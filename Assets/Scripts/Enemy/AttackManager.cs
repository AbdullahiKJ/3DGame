using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public LayerMask detectionMask;
    public LayerMask terrainLayer;
    public GameObject manager;
    public GameObject weapon;
    public bool canAttack = false;
    [SerializeField] AudioClip swingSoundFX;
    float minPitch = 0.8f;
    float maxPitch = 1.2f;

    void DisableAttack()
    {
        canAttack = false;
    }
    void EnableAttack()
    {
        canAttack = true;
    }
    void PlaySwingSoundFX()
    {
        SoundFXManager.instance.PlaySoundFXClip(swingSoundFX, transform, 0.3f, minPitch, maxPitch);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(weapon.transform.position, 1);
    }
}
