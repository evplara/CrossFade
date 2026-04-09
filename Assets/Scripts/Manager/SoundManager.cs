using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource referenceSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayAudio(AudioClip audioClip, Transform spawnPosition, float volume)
    {
        AudioSource audio = Instantiate(referenceSource, spawnPosition.position, Quaternion.identity);
        audio.clip = audioClip;
        audio.volume = volume;

        audio.Play();

        float length = audio.clip.length;
        Destroy(audio.gameObject, length);
    }

}
