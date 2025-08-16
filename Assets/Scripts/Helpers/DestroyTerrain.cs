using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DestroyTerrain : MonoBehaviour
{
    [SerializeField] GameObject fracturePrefab;
    GameObject fractureInstance;
    [SerializeField] float minForce = 100f;
    [SerializeField] float maxForce = 250f;
    float lifetime = 7f;
    float fadeDuration = 3f;
    [SerializeField] float defaultScale = 265f;
    [SerializeField] Material rockMat;
    Material sharedRockMat;
    [SerializeField] AudioClip[] soundFX;
    [SerializeField] float fxVolume = 0.5f;

    void Start()
    {
        sharedRockMat = new Material(rockMat);
    }

    public void TriggerExplosion(Vector3 attacker, float forceMultiplier = 1f)
    {
        // Hide mesh renderer and disable collider
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshRenderer.enabled = false;
        meshCollider.enabled = false;

        Quaternion rotation = transform.rotation;
        rotation.x = 0f;

        fractureInstance = Instantiate(fracturePrefab, transform.position, rotation);
        fractureInstance.transform.localScale = transform.localScale / defaultScale;

        foreach (Transform t in fractureInstance.transform)
        {
            Renderer renderer = t.GetComponent<Renderer>();
            renderer.sharedMaterial = sharedRockMat;

            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (transform.position - attacker).normalized;
                Vector3 force = direction * Random.Range(minForce, maxForce) * forceMultiplier;
                rb.AddForce(force);
            }
        }

        // Play the sound effect
        SoundFXManager.instance.PlayRandomSoundFXClip(soundFX, transform, fxVolume);

        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(lifetime);

        sharedRockMat.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                // Destroy the static and fractured game objects
                Destroy(fractureInstance);
                Destroy(this.gameObject);
            });
    }
}
