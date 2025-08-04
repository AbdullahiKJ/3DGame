using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSO", menuName = "Scriptable Objects/Damage")]
public class DamageSO : ScriptableObject
{
    public float baseDamage = 10f; // Amount of damage dealt
    public bool isFireDamage = false;
    public bool isIceDamage = false;
    public bool isElectricDamage = false;
    public float multiplier = 1f; // Multiplier for the damage, can be used to increase or decrease damage based on conditions
    public List<GameObject> specialEffectPrefabs; // Prefab for special effects when damage is dealt
}
