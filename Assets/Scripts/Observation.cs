using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Observation : MonoBehaviour
{
    [SerializeField] Volume slowVolume;
    float observationTimeLimit = 10f;
    float timer = 0f;
    bool observationTriggered = false;
    bool volumeTransitionTriggered = false;
    [SerializeField] float transitionSpeed = 0.1f;
    void Awake()
    {

    }

    void Update()
    {
        if (volumeTransitionTriggered) {
            float speed = transitionSpeed * (observationTriggered ? 1f : -1f);
            volumeTransition(speed);
        }
        // TODO: ADD CHECKS FOR MP METER
        //  if (observationTriggered) {
        //     if (timer > observationTimeLimit) {
        //         resetTimeScale();
        //         return;
        //     }
        //     timer += Time.deltaTime;
        //  }
    }

    // Reset the time scales and variables
    void resetTimeScale() {
        Time.timeScale = 1f;
        timer = 0f;
        observationTriggered = false;
    }

    // Trigger Observation and slow down time
    void OnObservation(InputValue value)
    {
        if(value.isPressed) {
            observationTriggered = !observationTriggered;
            Time.timeScale = observationTriggered ? 0.5f : 1f;
            volumeTransitionTriggered = true;
        }
    }

    void volumeTransition(float speed) {
        if (speed < 0f && slowVolume.weight > 0f) {
            slowVolume.weight += Time.fixedDeltaTime * speed;
        }
        else if (speed > 0f && slowVolume.weight < 1f ) {
            slowVolume.weight += Time.fixedDeltaTime * speed;
        }
        else {
            slowVolume.weight = observationTriggered ? 1f : 0f;
            volumeTransitionTriggered = false;
        }
    }
}
