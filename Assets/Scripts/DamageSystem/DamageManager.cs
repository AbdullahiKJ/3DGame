using System.Collections;
using DG.Tweening;
using Unity.UI.Shaders.Sample;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    Animator animator;
    public float maxHealth = 100f;
    public float currentHealth;
    // todo: add stagger reduction for the enemy if necessary
    // [SerializeField] float stagger = 100f;
    public bool isStaggering { get; set; } = false;
    [SerializeField] RangeBar healthBar;
    [SerializeField] GameObject hitParticlePrefab;
    public float blockThreshold = 0.5f; // Threshold for blocking damage
    [SerializeField] AudioClip[] defaultClips;
    [SerializeField] AudioClip blockSoundFX;

    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(Vector3 attacker, Vector3 contactPoint, DamageSO damageSO = null, float pushBack = 0f, bool knockback = false)
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

        // Optional block for the enemy to avoid damage
        if (this.gameObject.layer == LayerMask.NameToLayer("enemies"))
        {
            float blockChance = Random.Range(0f, 1f);
            if (blockChance > blockThreshold)
            {
                // Play sound effects
                SoundFXManager.instance.PlaySoundFXClip(blockSoundFX, transform, 0.5f, 0.8f, 1.2f);

                animator.SetTrigger("Block");
                StartCoroutine(WaitForStateTransition(attacker, 0f, false));
                return; // Blocked damage, exit early
            }
        }

        // Reduce health and stagger 
        float damageDealt = damageSO != null ? CalculateDamage(damageSO) : 10f;
        currentHealth -= damageDealt; // Example damage value
        healthBar.Value = new Vector2(1 - currentHealth / maxHealth, healthBar.Value.y);

        if (contactPoint != default(Vector3))
        {
            // Play hit particle effect
            if (damageSO.specialEffectPrefabs.Count == 0)
            {
                // Play base hit particle effect
                GameObject hitEffect = Instantiate(hitParticlePrefab, contactPoint, Quaternion.identity);
                Destroy(hitEffect, 2f); // Destroy the effect after 2 seconds
            }
            else
            {
                // Play each custom hit particle effect
                foreach (GameObject effectPrefab in damageSO.specialEffectPrefabs)
                {
                    GameObject hitEffect = Instantiate(effectPrefab, contactPoint, Quaternion.identity);
                    Destroy(hitEffect, 2f); // Destroy the effect after 2 seconds
                }
            }
        }

        // Play hit sounds
        AudioClip[] damageSOAudio = damageSO.audioClips.ToArray();
        if (damageSOAudio.Length > 0)
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(damageSOAudio, transform, 1f);
        }
        else
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(defaultClips, transform, 1f);
        }

        // Play the knockback animation if enabled
        if (knockback)
        {
            animator.Play("Knockback");
        }
        // Otherwise, play the stagger animation
        else
        {
            animator.Play("Small Stagger");
        }

        // Wait for the stagger animation to start
        StartCoroutine(WaitForStateTransition(attacker, pushBack, knockback));
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0f)
            currentHealth = 0f;
        healthBar.Value = new Vector2(1 - currentHealth / maxHealth, healthBar.Value.y);
    }

    // make the gameobject face the game object dealing damage
    public void FaceAttacker(Vector3 attacker, float pushBack, float animLength)
    {
        // Look at the target while charging (ignore the y-axis)
        Vector3 lookatTarget = attacker;
        lookatTarget.y = transform.position.y; // Ignore y-axis for rotation
        transform.LookAt(lookatTarget, Vector3.up);

        // Determine the distance to push back
        float posDiffMagnitude = new Vector2(attacker.x - transform.position.x, attacker.y - transform.position.y).magnitude;
        Vector3 pushDistance = transform.forward * -1;
        if (pushBack > 0f)
            pushDistance *= pushBack;
        else
            pushDistance *= posDiffMagnitude * 0.1f;

        // Move the character transform
        DOTween.To(() => transform.position, x => transform.position = x, transform.position + pushDistance, animLength)
            .SetEase(Ease.OutQuad);
    }

    // TODO: apply status effects based on the damageSO
    float CalculateDamage(DamageSO damageSO)
    {
        // Example calculation, can be replaced with more complex logic
        float damage = damageSO.baseDamage * damageSO.multiplier;
        if (damageSO.isFireDamage)
        {
            // Apply fire damage logic
        }
        if (damageSO.isIceDamage)
        {
            // Apply ice damage logic
        }
        if (damageSO.isElectricDamage)
        {
            // Apply poison damage logic
        }
        return damage;
    }

    IEnumerator WaitForStaggerAnimation(float time, bool knockback)
    {
        yield return new WaitForSeconds(time);

        // If the knockback animation is playing, wait for it to finish and start the getting up animation
        if (knockback)
        {
            // animator.Play("Getting Up");
            yield return null;

            // Check if there is an animator transition
            if (animator.IsInTransition(0))
            {
                yield return new WaitForSeconds(animator.GetAnimatorTransitionInfo(0).duration);
                yield return null;
            }

            float staggerLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.GetCurrentAnimatorStateInfo(0).speed;
            yield return new WaitForSeconds(staggerLength);
        }

        isStaggering = false;

        // Reset the block trigger
        if (this.gameObject.layer == LayerMask.NameToLayer("enemies"))
        {
            animator.ResetTrigger("Block");
        }
    }

    IEnumerator WaitForStateTransition(Vector3 attacker, float pushBack, bool knockback)
    {
        yield return null;

        // Check if there is an animator transition
        if (animator.IsInTransition(0))
        {
            yield return new WaitForSeconds(animator.GetAnimatorTransitionInfo(0).duration);
            yield return null;
        }

        float staggerLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.GetCurrentAnimatorStateInfo(0).speed;

        // Character cannot take damage for half of the stagger animation time
        StartCoroutine(WaitForStaggerAnimation(staggerLength, knockback));

        // face the attacker and move the game object to an appropriate distance from the attacker
        FaceAttacker(attacker, pushBack, staggerLength);
    }
}
