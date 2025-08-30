using System.Collections;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEngine.Events.UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();
    public MyFloatEvent OnAbilityStarted = new MyFloatEvent();

    [Header("Base Ability Settings")]
    public string title;
    public Sprite icon;
    public Sprite cooldownIcon;
    public Sprite cooldownIconOutline;
    public Color cooldownIconColor;
    public float cooldownTime = 5f;
    public float abilityDuration = 10f;
    public bool canUse = true;
    public bool abilityStarted = false;
    public GameObject hitParticlePrefab;
    AbilityManager abilityManager;

    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();
    }

    public void TriggerAbility()
    {
        if (canUse)
        {
            // Add the ability to the active abilities list in AbilityManager
            if (abilityManager != null)
            {
                abilityManager.activeAbilities.Add(title);
                abilityManager.UpdateAbilityIconColor();
            }
            Ability();
            StartCooldown();
        }
        else if (abilityManager != null)
        {
            // Play inactive sound for feedback
            abilityManager.PlayInactiveSound();
        }
    }

    public abstract void Ability();
    public abstract void EndAbility();
    public abstract void Helper();

    void StartCooldown()
    {
        canUse = false;
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            // Trigger the ability timer
            if (abilityDuration > 0)
            {
                OnAbilityStarted.Invoke(abilityDuration);
                yield return new WaitForSeconds(abilityDuration);
                EndAbility();
                abilityStarted = false;

                // Remove the ability from the active abilities list in AbilityManager
                if (abilityManager != null)
                {
                    abilityManager.activeAbilities.Remove(title);
                    abilityManager.ResetAbilityIconColor();
                }
            }

            // Trigger the cooldown timer
            OnAbilityUse.Invoke(cooldownTime);
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }
}
