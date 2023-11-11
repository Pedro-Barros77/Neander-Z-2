using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AudioExtensions
{
    public static IEnumerator Fade(this AudioSource source, float targetVolume, float durationMs)
    {
        float startVolume = source.volume;
        float startTime = Time.unscaledTime;
        float endTime = startTime + durationMs / 1000f;
        while (Time.unscaledTime < endTime)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, (Time.unscaledTime - startTime) / (durationMs / 1000f));
            yield return null;
        }
        source.volume = targetVolume;
    }

    /// <summary>
    /// Toca um áudio aleatório da lista, se houver algum, no AudioSource fornecido.
    /// </summary>
    /// <param name="audioList">A lista de audios.</param>
    /// <param name="audioSource">O AudioSource para tocar o som.</param>
    /// <param name="audioType">O tipo de audio a tocar, para ajustar o volume.</param>
    /// <returns>O audio aleatório selecionado.</returns>
    public static CustomAudio PlayRandomIfAny(this IEnumerable<CustomAudio> audioList, AudioSource audioSource, AudioTypes audioType)
    {
        if (audioList == null || !audioList.Any())
            return null;

        int randomIndex = Random.Range(0, audioList.Count());

        return audioList.PlayAtIndex(randomIndex, audioSource, audioType);
    }

    /// <summary>
    /// Toca um áudio da lista no index especificado, se houver algum, no AudioSource fornecido.
    /// </summary>
    /// <param name="audioList">A lista de audios.</param>
    /// <param name="index">O index do audio na lista a ser tocado.</param>
    /// <param name="audioSource">O AudioSource para tocar o som.</param>
    /// <param name="audioType">O tipo de audio a tocar, para ajustar o volume.</param>
    /// <returns>O audio selecionado.</returns>
    public static CustomAudio PlayAtIndex(this IEnumerable<CustomAudio> audioList, int index, AudioSource audioSource, AudioTypes audioType)
    {
        if (audioList == null || !audioList.Any() || index < 0 || index >= audioList.Count())
            return null;

        var clip = audioList.ElementAt(index);

        return clip.PlayIfNotNull(audioSource, audioType);
    }

    /// <summary>
    /// Toca o áudio fornecido, se existir, no AudioSource fornecido.
    /// </summary>
    /// <param name="audio">O audio a ser tocado.</param>
    /// <param name="audioSource">O AudioSource para tocar o som.</param>
    /// <param name="audioType">O tipo de audio a tocar, para ajustar o volume.</param>
    /// <returns>O audio selecionado.</returns>
    public static CustomAudio PlayIfNotNull(this CustomAudio audio, AudioSource audioSource, AudioTypes audioType)
    {
        if (audio == null)
            return null;

        float volumeMultiplier = audioType switch
        {
            AudioTypes.Music => MenuController.Instance.MusicVolume,
            AudioTypes.UI => MenuController.Instance.UIVolume,
            AudioTypes.Player => MenuController.Instance.PlayerVolume,
            AudioTypes.Enemies => MenuController.Instance.EnemiesVolume,
            _ => 1
        };

        audioSource.pitch = audio.Pitch;
        audioSource.PlayOneShot(audio.Audio, audio.Volume * volumeMultiplier);

        return audio;
    }

}
