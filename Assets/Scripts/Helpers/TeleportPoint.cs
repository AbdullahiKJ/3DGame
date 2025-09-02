using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    Teleport teleportScript;
    GameObject player;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] float minDistance = 2f;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    float intensity = 2f;
    Material material;
    MeshRenderer meshRenderer;

    void Awake()
    {
        teleportScript = FindFirstObjectByType<Teleport>();
        player = GameObject.Find("Y Bot");
        material = GetComponent<MeshRenderer>().material;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        // Change the material color depending on the teleport cooldown and distance from the player
        if (teleportScript.canUse && distance < maxDistance && distance > 8f)
        {
            meshRenderer.enabled = true;
            material.SetColor("_EmissionColor", activeColor * intensity);
        }
        else if (distance < minDistance)
        {
            // Hide the teleport point if too close to the player
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
            material.SetColor("_EmissionColor", inactiveColor * intensity);
        }
    }

    // run when object comes into view of camera
    void OnBecameVisible()
    {
        // check if in list and add
        if (!Targets.Instance.HasTarget(gameObject, teleportScript.teleportTargets))
        {
            Targets.Instance.AddTarget(gameObject, teleportScript.teleportTargets);
        }
    }

    void OnBecameInvisible()
    {
        if (Targets.Instance.HasTarget(gameObject, teleportScript.teleportTargets))
        {
            Targets.Instance.RemoveTarget(gameObject, teleportScript.teleportTargets);
        }
    }
}
