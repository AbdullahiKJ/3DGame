using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public LayerMask detectionMask;
    public LayerMask terrainLayer;
    public GameObject manager;
    public GameObject weapon;
    public bool canAttack = false;
    [SerializeField] AudioClip swingSoundFX;
    float clipLength;
    float minPitch = 0.8f;
    float maxPitch = 1.2f;
    float minLength;
    float maxLength;

    void Start()
    {
        clipLength = swingSoundFX.length;
        minLength = clipLength / minPitch;
        maxLength = clipLength / maxPitch;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlaySwingSoundFX();
    }
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
        float rand = Random.Range(minLength, maxLength);
        SoundFXManager.instance.PlaySoundFXClip(swingSoundFX, transform, 1f, rand);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(weapon.transform.position, 1);
    }
}
