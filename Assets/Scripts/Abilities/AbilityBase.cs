using System.Collections;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEngine.Events.UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();
    public MyFloatEvent OnAbilityStarted = new MyFloatEvent();

    [Header("Ability Settings")]
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

    public void TriggerAbility()
    {
        if (canUse)
        {
            Ability();
            StartCooldown();
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
            }

            // Trigger the cooldown timer
            OnAbilityUse.Invoke(cooldownTime);
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }
}
