using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource AudioSource { get; set; }

    [SerializeField]
    public List<CustomAudio> AudioClips;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Toca um audio aleatório da lista de audios.
    /// </summary>
    public void PlayRandomAudio()
    {
        var customAudio = AudioClips[Random.Range(0, AudioClips.Count)];
        AudioSource.pitch = customAudio.Pitch;
        AudioSource.PlayOneShot(customAudio.Audio, customAudio.Volume);
    }

    /// <summary>
    /// Toca um audio da lista de audios, no index definido.
    /// </summary>
    /// <param name="index">O index do audio na lista a ser executado.</param>
    public void PlayAudio(int index)
    {
        var customAudio = AudioClips[index];
        AudioSource.pitch = customAudio.Pitch;
        if (index < AudioClips.Count)
            AudioSource.PlayOneShot(customAudio.Audio, customAudio.Volume);
    }

    /// <summary>
    /// Destroi o objeto após o tempo definido em segundos.
    /// </summary>
    /// <param name="timeDelay"></param>
    public void AutoDestroy(float timeDelay)
    {
        Destroy(gameObject, timeDelay);
    }
}

[System.Serializable]
public class CustomAudio
{
    public AudioClip Audio;
    public float Volume;
    public float Pitch;
}
