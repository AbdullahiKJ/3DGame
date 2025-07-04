using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public AbilityBase[] abilities;
    public enum AbilityType
    {
        None,
        Ability1,
        Ability2,
        Ability3,
        Ability4
    }
    AbilityType currentAbility;
    public AbilityBase CurrentAbilityScript =>
    ((int)currentAbility - 1 >= 0 && (int)currentAbility - 1 < abilities.Length)
        ? abilities[(int)currentAbility - 1]
        : null;
    bool canUseAbility = true;

    void Awake()
    {
        abilities = GetComponents<AbilityBase>();
        if (abilities.Length == 0)
        {
            currentAbility = AbilityType.None;
        }
        else
        {
            currentAbility = AbilityType.Ability1; // Default to the first ability
        }
    }

    // Update is called once per frame
    void Update()
    {
        CurrentAbilityScript?.Helper(); // Call the helper method if it exists
    }

    void OnAbilityUse(InputValue value)
    {
        Debug.Log("Button pressed");
        if (value.isPressed && currentAbility != AbilityType.None && canUseAbility)
        {
            Debug.Log("Can use ability");
            int abilityIndex = (int)currentAbility - 1; // Convert enum to index
            if (abilityIndex >= 0 && abilityIndex < abilities.Length)
            {
                Debug.Log("Ability exists");
                abilities[abilityIndex].TriggerAbility();
            }
        }
    }
}
