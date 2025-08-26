using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class ReverseTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] float reverseSpeed = 1f;
    [SerializeField] AudioClip reverseClip;
    [SerializeField] Volume reverseVolume;
    bool startedReverse = false;

    void Update()
    {
        if (director != null && director.state == PlayState.Playing && startedReverse)
        {
            // Move time backwards
            director.time -= Time.deltaTime * reverseSpeed;

            // Stop at beginning
            if (director.time <= 0)
            {
                director.time = 0;
                director.Stop();
                startedReverse = false;

                // Start the main playable scene
                GameDirector.instance.StartPlayableScene();

                // Transition out of the reverse global volume
                if (reverseVolume != null)
                    DOTween.To(() => reverseVolume.weight, x => reverseVolume.weight = x, 0f, 0.5f);
            }
        }
    }

    public void PlayReverse()
    {
        startedReverse = true;
        // Ensure timeline is at the end
        director.time = director.duration;
        director.Play();

        // Play sound fx
        float sequenceLength = (float)director.playableAsset.duration / reverseSpeed;
        SoundFXManager.instance.PlaySoundFXClip(reverseClip, transform, 1f, sequenceLength);

        // Transition to the reverse global volume
        if (reverseVolume != null)
            DOTween.To(() => reverseVolume.weight, x => reverseVolume.weight = x, 1f, 0.5f);
    }
}
