using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlameArmament : AbilityBase
{
    [Header("Flame Armament Settings")]
    [SerializeField] GameObject flameVfxPrefab;
    [SerializeField] GameObject[] vfxParents;
    [SerializeField] SkinnedMeshRenderer surfaceMeshRenderer;
    [SerializeField] Material outlineMaterial;
    List<GameObject> flameVfxInstances = new List<GameObject>();
    [SerializeField] DamageSO damageSO;
    [SerializeField] float damageMultiplier = 1.5f;
    [SerializeField] UniversalRendererData overlayRenderer;
    ScriptableRendererFeature overlayFeature;

    void Awake()
    {
        // Disable the overlay feature for the flame effect
        if (overlayFeature != null)
        {
            overlayFeature.SetActive(false);
        }
    }

    public override void Ability()
    {
        // Create flame VFX instances at each parent object
        foreach (GameObject parent in vfxParents)
        {
            GameObject newInstance = Instantiate(flameVfxPrefab, parent.transform);
            flameVfxInstances.Add(newInstance);
        }
        // Add the outline material to the surface mesh renderer
        if (surfaceMeshRenderer != null && outlineMaterial != null)
        {
            Material[] materials = surfaceMeshRenderer.materials;
            List<Material> materialList = new List<Material>(materials);
            materialList.Add(outlineMaterial);
            surfaceMeshRenderer.materials = materialList.ToArray();
        }

        // Add damage modifier to the player
        damageSO.multiplier *= damageMultiplier;

        // Add fire hit fx to the damage scriptable object
        if (damageSO.specialEffectPrefabs.Find(prefab => prefab.name.StartsWith(hitParticlePrefab.name)) == null)
        {
            damageSO.specialEffectPrefabs.Add(hitParticlePrefab);
        }

        // Enable the overlay feature for the flame effect
        if (overlayRenderer != null)
        {
            overlayFeature = overlayRenderer.rendererFeatures.Find(feature => feature is FullScreenPassRendererFeature);
            if (overlayFeature != null)
                overlayFeature.SetActive(true);
        }

        abilityStarted = true;
    }

    public override void Helper()
    {
    }

    public override void EndAbility()
    {
        // Destroy all flame VFX instances
        foreach (GameObject instance in flameVfxInstances)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        flameVfxInstances.Clear();

        // Remove the outline material from the surface mesh renderer
        if (surfaceMeshRenderer != null && outlineMaterial != null)
        {
            Material[] materials = surfaceMeshRenderer.materials;
            List<Material> materialList = new List<Material>(materials);
            materialList.RemoveAll(mat => mat.name.StartsWith(outlineMaterial.name));
            surfaceMeshRenderer.materials = materialList.ToArray();
        }

        // Reset the damage multiplier
        damageSO.multiplier /= damageMultiplier;

        // Remove fire hit fx from the damage scriptable object
        damageSO.specialEffectPrefabs.RemoveAll(prefab => prefab.name.StartsWith(hitParticlePrefab.name));

        // Disable the overlay feature for the flame effect
        if (overlayFeature != null)
        {
            overlayFeature.SetActive(false);
        }

    }
}
