using System.Collections;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    [Header("Ability Settings")]
    public string title;
    public Sprite icon;
    public float cooldownTime = 1f;
    private bool canUse = true;


    void StartCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            canUse = false;
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }
}
