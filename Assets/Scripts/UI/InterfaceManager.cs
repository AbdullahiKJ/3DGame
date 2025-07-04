using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] AbilityManager player;
    [SerializeField] Transform abilityUiParent;
    [SerializeField] AbilityUI abilityUIPrefab;
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
            AbilityUI abilityUI = Instantiate(abilityUIPrefab, abilityUiParent);
            player.abilities[i].OnAbilityUse.AddListener((cooldown) => abilityUI.ShowCoolDown(cooldown));
            abilityUI.SetIcon(player.abilities[i].icon);
        }

    }
}
