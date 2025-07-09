using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSO", menuName = "Scriptable Objects/Damage")]
public class DamageSO : ScriptableObject
{
    public float baseDamage = 10f; // Amount of damage dealt
    // TODO: Add elemental damage and status effects
    public bool isFireDamage = false; // Whether the damage is fire-based
    public bool isIceDamage = false; // Whether the damage is ice-based
    public bool isPoisonDamage = false; // Whether the damage is poison-based
    public float multiplier = 1f; // Multiplier for the damage, can be used to increase or decrease damage based on conditions
    public List<GameObject> specialEffectPrefabs; // Prefab for special effects when damage is dealt
}
