using UnityEngine;
using UnityEngine.Playables;

public class ReverseTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] float reverseSpeed = 1f;
    [SerializeField] AudioClip reverseClip;
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

                // Start the main playable scene
                GameDirector.instance.StartPlayableScene();
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
    }
}
