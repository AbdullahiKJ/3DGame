using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] AudioSource soundFXPrefab;
    [SerializeField] AudioSource ambientPrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null)
            return;

        AudioSource audioSource = Instantiate(soundFXPrefab, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = clip.length;
        Destroy(audioSource, clipLength);
    }
    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, clips.Length);
        PlaySoundFXClip(clips[rand], spawnTransform, volume);
    }
    public void PlayAmbientClip(AudioClip clip, Transform spawnTransform, float volume, float duration)
    {
        if (clip == null)
            return;
        AudioSource audioSource = Instantiate(ambientPrefab, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, duration);
    }

    // todo: get audio clips for attacks, vfx, collisions, background noise, music
    // todo: attach where necessary
}
