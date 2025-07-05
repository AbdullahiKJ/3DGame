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
    [SerializeField] RectTransform uiSelector;

    (int, int)[] abilityUiPositions = new (int, int)[]
    {
        (25,25), // Ability 1
        (25,-25), // Ability 2
        (-25,-25), // Ability 3
        (-25,25)  // Ability 4
    };

    void Awake()
    {
        abilities = GetComponents<AbilityBase>();
        if (abilities.Length == 0)
        {
            currentAbility = AbilityType.None;
            uiSelector.gameObject.SetActive(false);
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
        if (value.isPressed && currentAbility != AbilityType.None && canUseAbility)
        {
            int abilityIndex = (int)currentAbility - 1; // Convert enum to index
            if (abilityIndex >= 0 && abilityIndex < abilities.Length)
            {
                abilities[abilityIndex].TriggerAbility();
            }
        }
    }

    void OnAbilitySelect(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input == new Vector2(0, 1) && abilities.Length > 0)
        {
            currentAbility = AbilityType.Ability1;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[0].Item1, abilityUiPositions[0].Item2);
        }
        else if (input == new Vector2(1, 0) && abilities.Length > 1)
        {
            currentAbility = AbilityType.Ability2;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[1].Item1, abilityUiPositions[1].Item2);
        }
        else if (input == new Vector2(0, -1) && abilities.Length > 2)
        {
            currentAbility = AbilityType.Ability3;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[2].Item1, abilityUiPositions[2].Item2);
        }
        else if (input == new Vector2(-1, 0) && abilities.Length > 3)
        {
            currentAbility = AbilityType.Ability4;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[3].Item1, abilityUiPositions[3].Item2);
        }
    }
}
