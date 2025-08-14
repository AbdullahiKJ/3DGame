using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] DamageSO damageSO;
    [SerializeField] LayerMask playerLayerMask;
    float growDuration = 2f; // Duration for the lava to grow
    float shrinkDuration = 2f; // Duration for the lava to shrink
    float holdDuration = 5f; // Duration for the lava to hold its size 
    float initialScale = 0.1f; // Initial scale of the lava
    Vector3 holdScale = Vector3.one * 4.5f; // Scale when the lava is fully grown
    bool isOnLava = false;
    float timer = 0f;
    float damageInterval = 2f; // Time interval for dealing damage
    DamageManager damageManager;

    void Start()
    {
        transform.localScale = Vector3.one * initialScale; // Set initial scale
        DOTween.To(() => transform.localScale, x => transform.localScale = x, holdScale, growDuration)
            .OnComplete(() => StartCoroutine(WaitForHoldAndShrink())); // Start hold timer
    }

    void Update()
    {
        if (isOnLava)
        {
            timer += Time.deltaTime;
            if (timer >= damageInterval)
            {
                DealDamage();
                timer = 0f; // Reset timer after dealing damage
            }
        }
    }

    IEnumerator WaitForHoldAndShrink()
    {
        yield return new WaitForSeconds(holdDuration);
        DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.zero, shrinkDuration)
            .OnComplete(() => Destroy(gameObject)); // Destroy the lava object after shrinking
    }

    void OnTriggerEnter(Collider other)
    {
        isOnLava = true;
        damageManager = other.gameObject.GetComponent<DamageManager>();
        DealDamage();
    }

    void OnTriggerExit(Collider other)
    {
        isOnLava = false;
        timer = 0f; // Reset timer when exiting lava
    }

    void DealDamage()
    {
        if (damageManager != null)
            damageManager.TakeDamage(transform.position, default, damageSO);
    }
}
