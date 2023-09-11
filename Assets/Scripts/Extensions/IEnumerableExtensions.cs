using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static string JoinString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }

    public static CustomAudio PlayRandomIfAny(this IEnumerable<CustomAudio> audioList, AudioSource audioSource)
    {
        if (audioList == null || !audioList.Any())
            return null;

        var randomClip = audioList.ElementAt(Random.Range(0, audioList.Count()));
        audioSource.pitch = randomClip.Pitch;
        audioSource.PlayOneShot(randomClip.Audio, randomClip.Volume);

        return randomClip;
    }
}
