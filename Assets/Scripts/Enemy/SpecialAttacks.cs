using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SpecialAttacks : MonoBehaviour
{
    [Header("Lightning Strike Settings")]
    [SerializeField] GameObject lightningStrikePrefab;
    [SerializeField] Transform strikePosition;
    GameObject lightningStrikeInstance;

    public void TriggerLightningStrike()
    {
        if (lightningStrikePrefab != null)
        {
            lightningStrikeInstance = Instantiate(lightningStrikePrefab, strikePosition.position, Quaternion.identity);
            float strikeDuration = lightningStrikeInstance.GetComponent<VisualEffect>().GetFloat("Lifetime");
            StartCoroutine(WaitForStrikeEnd(strikeDuration));
        }
    }

    IEnumerator WaitForStrikeEnd(float duration)
    {
        // Wait for the lightning strike to finish
        yield return new WaitForSeconds(duration);
        // Destroy the lightning strike instance if it exists
        if (lightningStrikeInstance != null)
            Destroy(lightningStrikeInstance);
    }
}
