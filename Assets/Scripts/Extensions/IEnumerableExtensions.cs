using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Verifica se a lista é nula ou vazia.
    /// </summary>
    /// <typeparam name="T">O tipo da lista.</typeparam>
    /// <param name="source">A lista.</param>
    /// <returns>True se a lista é nula ou está vazia, senão, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Retorna a lista ou um valor padrão caso a lista seja nula ou vazia.
    /// </summary>
    /// <typeparam name="T">O tipo da lista.</typeparam>
    /// <param name="source">A lista.</param>
    /// <param name="value">O valor padrão a ser devolvido caso a lista seja nula ou vazia.</param>
    /// <returns>A lista ou o valor padrão.</returns>
    public static IEnumerable<T> ValueOrDefault<T>(this IEnumerable<T> source, IEnumerable<T> value)
    {
        return source.IsNullOrEmpty() ? value : source;
    }

    /// <summary>
    /// Retorna a string ou um valor padrão caso a string seja nula ou vazia.
    /// </summary>
    /// <param name="source">A string.</param>
    /// <param name="value">O valor padrão a ser devolvido caso a string seja nula ou vazia.</param>
    /// <returns>A string ou o valor padrão.</returns>
    public static string ValueOrDefault(this string source, string value)
    {
        return source.IsNullOrEmpty() ? value : source;
    }

    /// <summary>
    /// Junta os elementos da lista em uma string, separados pelo separador especificado.
    /// </summary>
    /// <typeparam name="T">O tipo da lista.</typeparam>
    /// <param name="source">A lista.</param>
    /// <param name="separator">O separador.</param>
    /// <returns>Uma string contendo todos os elementos da lista separados pelo separador.</returns>
    public static string JoinString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
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
