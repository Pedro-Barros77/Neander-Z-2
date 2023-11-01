using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource AudioSource { get; set; }
    public AudioTypes AudioType;

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
        AudioClips.PlayRandomIfAny(AudioSource, AudioType);
    }

    /// <summary>
    /// Toca um audio da lista de audios, no index definido.
    /// </summary>
    /// <param name="index">O index do audio na lista a ser executado.</param>
    public void PlayAudio(int index)
    {
        AudioClips.PlayAtIndex(index, AudioSource, AudioType);
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
