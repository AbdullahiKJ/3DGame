using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lightning : AbilityBase
{
    [Header("Lightning Settings")]
    [SerializeField] RuntimeAnimatorController baseController;
    [SerializeField] RuntimeAnimatorController lightningController;
    Animator animator;
    PlayerInput playerInput;
    Movement baseMovementScript;
    Combat combatScript;
    LightningMovement lightningMovementScript;
    [SerializeField] SkinnedMeshRenderer surfaceMeshRenderer;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject auraVfxPrefab;
    GameObject auraVfxInstance;
    [SerializeField] GameObject crossHair;
    [SerializeField] AudioClip soundFX;
    [SerializeField] AudioClip ambientSound;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        baseMovementScript = GetComponent<Movement>();
        combatScript = GetComponent<Combat>();
        lightningMovementScript = GetComponent<LightningMovement>();
        lightningMovementScript.enabled = false;
        crossHair.SetActive(false);
    }
    public float lightningDamage = 10f;

    public override void Helper()
    {
    }

    public override void Ability()
    {
        // Disable the base movement script and combat script
        baseMovementScript.enabled = false;
        combatScript.enabled = false;

        // Enable the lightning movement script
        lightningMovementScript.enabled = true;

        // Switch over to the lightning animator controller
        animator.runtimeAnimatorController = lightningController;

        // Switch the player input to the lightning action map
        playerInput.SwitchCurrentActionMap("Lightning");

        // Add the outline material to the surface mesh renderer
        if (surfaceMeshRenderer != null && outlineMaterial != null)
        {
            Material[] materials = surfaceMeshRenderer.materials;
            List<Material> materialList = new List<Material>(materials);
            materialList.Add(outlineMaterial);
            surfaceMeshRenderer.materials = materialList.ToArray();
        }

        // Instantiate the lightning aura VFX
        if (auraVfxPrefab != null)
            auraVfxInstance = Instantiate(auraVfxPrefab, this.transform);

        // Enable the crosshair
        crossHair.SetActive(true);

        // Play the ambient sound and sound FX
        SoundFXManager.instance.PlaySoundFXClip(soundFX, transform, 1f);
        SoundFXManager.instance.PlayAmbientClip(ambientSound, transform, 1f, this.abilityDuration);

        abilityStarted = true;
    }
    public override void EndAbility()
    {
        // Disable the lightning movement script
        lightningMovementScript.enabled = false;

        // Re-enable the base movement script and combat script
        baseMovementScript.enabled = true;
        combatScript.enabled = true;

        // Switch back to the base animator controller
        animator.runtimeAnimatorController = baseController;

        // Switch back to the default player input action map
        playerInput.SwitchCurrentActionMap("Player");


        // Remove the outline material from the surface mesh renderer
        if (surfaceMeshRenderer != null && outlineMaterial != null)
        {
            Material[] materials = surfaceMeshRenderer.materials;
            List<Material> materialList = new List<Material>(materials);
            materialList.RemoveAll(mat => mat.name.StartsWith(outlineMaterial.name));
            surfaceMeshRenderer.materials = materialList.ToArray();
        }

        // Destroy the lightning aura VFX instance
        if (auraVfxInstance != null)
            Destroy(auraVfxInstance);

        // Disable the crosshair
        crossHair.SetActive(false);
    }

}
