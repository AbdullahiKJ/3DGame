using System.Collections.Generic;
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
    AbilityType selectedAbility;
    public AbilityBase CurrentAbilityScript =>
    ((int)selectedAbility - 1 >= 0 && (int)selectedAbility - 1 < abilities.Length)
        ? abilities[(int)selectedAbility - 1]
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
    public List<string> activeAbilities = new List<string>();
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] AudioClip inactiveSound;

    void Awake()
    {
        abilities = GetComponents<AbilityBase>();
        if (abilities.Length == 0)
        {
            selectedAbility = AbilityType.None;
            uiSelector.gameObject.SetActive(false);
        }
        else
        {
            selectedAbility = AbilityType.Ability1; // Default to the first ability
        }
    }

    // Update is called once per frame
    void Update()
    {
        int abilityIndex = (int)selectedAbility - 1; // Convert enum to index
        if (IsValidCombination(abilities[abilityIndex].title))
        {
            CurrentAbilityScript?.Helper(); // Call the helper method if it exists and is a valid combination
        }
    }

    void OnAbilityUse(InputValue value)
    {
        if (value.isPressed && selectedAbility != AbilityType.None && canUseAbility)
        {
            int abilityIndex = (int)selectedAbility - 1; // Convert enum to index
            if (abilityIndex >= 0 && abilityIndex < abilities.Length && IsValidCombination(abilities[abilityIndex].title))
            {
                abilities[abilityIndex].TriggerAbility();
            }
            else
            {
                // Play invalid ability sound for feedback
                PlayInactiveSound();
            }
        }
    }

    void OnAbilitySelect(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input == new Vector2(0, 1) && abilities.Length > 0)
        {
            selectedAbility = AbilityType.Ability1;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[0].Item1, abilityUiPositions[0].Item2);
        }
        else if (input == new Vector2(1, 0) && abilities.Length > 1)
        {
            selectedAbility = AbilityType.Ability2;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[1].Item1, abilityUiPositions[1].Item2);
        }
        else if (input == new Vector2(0, -1) && abilities.Length > 2)
        {
            selectedAbility = AbilityType.Ability3;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[2].Item1, abilityUiPositions[2].Item2);
        }
        else if (input == new Vector2(-1, 0) && abilities.Length > 3)
        {
            selectedAbility = AbilityType.Ability4;
            uiSelector.anchoredPosition = new Vector2(abilityUiPositions[3].Item1, abilityUiPositions[3].Item2);
        }
    }

    // Check if the ability combination is valid
    bool IsValidCombination(string ability)
    {
        if (activeAbilities.Contains("Lightning"))
        {
            if (ability == "Flame Armament" || ability == "Teleport")
            {
                return false; // Invalid combination
            }
        }
        else if (activeAbilities.Contains("Flame Armament"))
        {
            if (ability == "Lightning")
            {
                return false; // Invalid combination
            }
        }
        return true; // Valid combination
    }

    public void UpdateAbilityIconColor()
    {
        AbilityUI[] abilityUIs = FindObjectsByType<AbilityUI>(FindObjectsSortMode.None);
        foreach (var ui in abilityUIs)
        {
            if (!IsValidCombination(ui.title))
            {
                ui.SetColour(inactiveColor);
            }
        }
    }

    public void ResetAbilityIconColor()
    {
        AbilityUI[] abilityUIs = FindObjectsByType<AbilityUI>(FindObjectsSortMode.None);
        foreach (var ui in abilityUIs)
        {
            ui.SetColour(activeColor);
        }
    }

    public void PlayInactiveSound()
    {
        // todo: add sound effect in the inspector
        SoundFXManager.instance.PlaySoundFXClip(inactiveSound, transform, 1f);
    }
}
