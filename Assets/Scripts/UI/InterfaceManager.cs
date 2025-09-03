using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] AbilityManager player;
    [SerializeField] Transform abilityUiParent;
    [SerializeField] AbilityUI abilityUIPrefab;
    [SerializeField] AbilityUI abilityCooldownPrefab;
    (int, int)[] abilityUiPositions = new (int, int)[]
    {
        (25,25), // Ability 1
        (25,-25), // Ability 2
        (-25,-25), // Ability 3
        (-25,25)  // Ability 4
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < player.abilities.Length; i++)
        {
            // Instantiate abilty UI and set its properties
            AbilityUI abilityUI = Instantiate(abilityUIPrefab, abilityUiParent);
            player.abilities[i].OnAbilityUse.AddListener((cooldown) => abilityUI.ShowCoolDown(cooldown));
            abilityUI.SetIcon(player.abilities[i].icon);
            abilityUI.SetTransform(abilityUiPositions[i]);
            abilityUI.title = player.abilities[i].title;

            // Instantiate ability cooldown UI and set its properties
            if (player.abilities[i].abilityDuration > 0f)
            {
                int index = i;
                AbilityUI abilityCooldownUI = Instantiate(abilityCooldownPrefab, this.gameObject.transform);
                player.abilities[i].OnAbilityStarted.AddListener((cooldown) =>
                {
                    abilityCooldownUI.gameObject.SetActive(true);
                    abilityCooldownUI.SetIcon(player.abilities[index].cooldownIcon);
                    abilityCooldownUI.SetOutlineIcon(player.abilities[index].cooldownIconOutline);
                    abilityCooldownUI.SetColour(player.abilities[index].cooldownIconColor);
                    abilityCooldownUI.ShowCoolDown(cooldown, true);
                });
                player.abilities[i].OnAbilityCancelled.AddListener(() =>
                {
                    abilityCooldownUI.gameObject.SetActive(false);
                });
                abilityCooldownUI.gameObject.SetActive(false);
            }
        }

    }
}
