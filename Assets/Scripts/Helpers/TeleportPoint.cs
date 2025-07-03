using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    Teleport teleportScript;
    GameObject player;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    float intensity = 5f;
    Material material;

    void Awake()
    {
        teleportScript = FindFirstObjectByType<Teleport>();
        player = GameObject.Find("Y Bot");
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        // Change the material color depending on the teleport cooldown and distance from the player
        if (teleportScript.getCanTeleport() && distance < maxDistance && distance > 8f)
        {
            material.SetColor("_EmissionColor", activeColor * intensity);
        }
        else
        {
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
