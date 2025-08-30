using System.Collections;
using DG.Tweening;
using Unity.UI.Shaders.Sample;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Observation : MonoBehaviour
{
    [SerializeField] Volume slowVolume;
    [SerializeField] float observationTimeLimit = 7f;
    float timer = 0f;
    bool observationTriggered = false;
    [SerializeField] float transitionSpeed = 0.1f;
    Animator animator;
    [SerializeField] AudioClip startSoundFX;
    [SerializeField] AudioClip endSoundFX;
    [SerializeField] CustomSlider uiSlider;
    bool holdingTimer = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        uiSlider.Value = 1f;
        timer = observationTimeLimit;
    }

    void Update()
    {
        // Do nothing while the timer is on hold
        if (holdingTimer)
        {
            return;
        }
        // Decrease the timer while observation is active
        else if (observationTriggered)
        {
            if (timer < 0f)
            {
                timer = 0f;
                uiSlider.Value = 0f;
                DOTween.To(() => slowVolume.weight, x => slowVolume.weight = x, 0f, 0.5f);
                resetTimeScale();
                StartCoroutine(HoldTimer());
                return;
            }
            // Account for time scale when decreasing the timer
            timer -= Time.deltaTime / Time.timeScale;
            uiSlider.Value = timer / observationTimeLimit;
        }
        // Increase the timer while observation is inactive
        else
        {
            if (timer > observationTimeLimit)
            {
                timer = observationTimeLimit;
                uiSlider.Value = 1f;
                return;
            }
            timer += Time.deltaTime;
            uiSlider.Value = timer / observationTimeLimit;
        }
    }

    // Trigger Observation and slow down time
    void OnObservation(InputValue value)
    {
        if (value.isPressed && timer > 0f)
        {
            // Toggle observation state and time scale
            observationTriggered = !observationTriggered;
            Time.timeScale = observationTriggered ? 0.5f : 1f;

            // Trigger volume transition
            DOTween.To(() => slowVolume.weight, x => slowVolume.weight = x, observationTriggered ? 1f : 0f, 0.5f);

            animator.SetFloat("timeScaleMultiplier", 1 / Time.timeScale);

            // Play the appropriate sound effects
            if (observationTriggered)
            {
                SoundFXManager.instance.PlaySoundFXClip(startSoundFX, transform, 1f);
            }
            else
            {
                // Play the start sound fx in reverse
                SoundFXManager.instance.PlaySoundFXClip(endSoundFX, transform, 1f, 2f, 2f);
            }

            // Apply a hold on the observation timer if it has been turned off
            if (!observationTriggered)
            {
                StartCoroutine(HoldTimer());
            }
        }
    }

    // Reset the time scales and variables
    void resetTimeScale()
    {
        Time.timeScale = 1f;
        timer = 0f;
        observationTriggered = false;
        animator.SetFloat("timeScaleMultiplier", 1 / Time.timeScale);
    }

    IEnumerator HoldTimer()
    {
        holdingTimer = true;
        yield return new WaitForSeconds(3f);
        holdingTimer = false;
    }
}
