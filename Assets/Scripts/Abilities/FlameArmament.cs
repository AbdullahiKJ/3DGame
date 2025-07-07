using System.Collections.Generic;
using UnityEngine;

public class FlameArmament : AbilityBase
{
    [SerializeField] GameObject flameVfxPrefab;
    [SerializeField] GameObject[] vfxParents;
    [SerializeField] SkinnedMeshRenderer surfaceMeshRenderer;
    [SerializeField] Material outlineMaterial;
    List<GameObject> flameVfxInstances = new List<GameObject>();
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

        // Add fire hit fx to the player

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
    }
}
