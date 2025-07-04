using System.Collections;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEngine.Events.UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();

    [Header("Ability Settings")]
    public string title;
    public Sprite icon;
    public float cooldownTime = 5f;
    public bool canUse = true;
    public bool abilityStarted = false;

    public void TriggerAbility()
    {
        Debug.Log("Base script triggered");
        if (canUse)
        {
            Debug.Log("Main ability function called");
            Ability();
            StartCooldown();
        }
    }

    public abstract void Ability();
    public abstract void Helper();

    void StartCooldown()
    {
        OnAbilityUse.Invoke(cooldownTime);
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            canUse = false;
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }

    // todo: if needed, create an event for when the abilty is used and subscribe the start cooldown function to it
}
