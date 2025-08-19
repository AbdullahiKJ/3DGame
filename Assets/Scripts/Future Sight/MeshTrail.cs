using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] float meshRefreshRate = 0.1f;
    [SerializeField] float meshDestroyDelay = 3f;
    [SerializeField] Transform spawnPosition;
    bool isTrailActive = false;
    SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("Material")]
    [SerializeField] Material trailMaterial;
    [SerializeField] string shaderVarRef = "_Alpha";
    [SerializeField] string shaderVarColourRef = "_Colour";
    [SerializeField] float shaderVarRate = 0.1f;
    [SerializeField] float shaderVarRefreshRate = 0.05f;
    float hue = 0f;
    float maxHueValue = 1f;

    // todo: remove the start function for the main function
    // todo: disable the script if enabled
    void Start()
    {
        isTrailActive = true;
        StartCoroutine(ActivateTrail());
    }

    void OnEnable()
    {
        isTrailActive = true;
        StartCoroutine(ActivateTrail());
    }
    void OnDisable()
    {
        isTrailActive = false;
        StopAllCoroutines();
    }

    public void EnableScript()
    {
        this.enabled = true;
    }

    public void DisableScript()
    {
        this.enabled = false;
    }

    IEnumerator ActivateTrail()
    {
        while (isTrailActive)
        {
            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            // Get the current colour
            hue += meshRefreshRate;
            if (hue > maxHueValue)
                hue %= maxHueValue;
            Color meshColour = Color.HSVToRGB(hue, maxHueValue, maxHueValue);

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject trail = new GameObject();
                trail.transform.SetPositionAndRotation(spawnPosition.transform.position, spawnPosition.rotation);

                MeshRenderer mr = trail.AddComponent<MeshRenderer>();
                MeshFilter mf = trail.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = trailMaterial;
                mr.material.SetColor(shaderVarColourRef, meshColour);

                StartCoroutine(AnimateMaterialFloat(mr.material, 0f));

                Destroy(trail, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= shaderVarRate;
            mat.SetFloat(shaderVarRef, valueToAnimate);

            yield return new WaitForSeconds(shaderVarRefreshRate);
        }
    }
}
