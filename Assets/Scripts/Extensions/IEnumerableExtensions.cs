using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Verifica se a lista � nula ou vazia.
    /// </summary>
    /// <typeparam name="T">O tipo da lista.</typeparam>
    /// <param name="source">A lista.</param>
    /// <returns>True se a lista � nula ou est� vazia, sen�o, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Retorna a lista ou um valor padr�o caso a lista seja nula ou vazia.
    /// </summary>
    /// <typeparam name="T">O tipo da lista.</typeparam>
    /// <param name="source">A lista.</param>
    /// <param name="value">O valor padr�o a ser devolvido caso a lista seja nula ou vazia.</param>
    /// <returns>A lista ou o valor padr�o.</returns>
    public static IEnumerable<T> ValueOrDefault<T>(this IEnumerable<T> source, IEnumerable<T> value)
    {
        return source.IsNullOrEmpty() ? value : source;
    }

    /// <summary>
    /// Retorna a string ou um valor padr�o caso a string seja nula ou vazia.
    /// </summary>
    /// <param name="source">A string.</param>
    /// <param name="value">O valor padr�o a ser devolvido caso a string seja nula ou vazia.</param>
    /// <returns>A string ou o valor padr�o.</returns>
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
    public static string JoinString<T>(this IEnumerable<T> source, string separator = ",")
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// Retorna um elemento aleat�rio da lista.
    /// </summary>
    /// <typeparam name="T">O tipo do elemento na lista.</typeparam>
    /// <param name="source">A lista fonte.</param>
    /// <returns>Um elemento aleat�rio da lista.</returns>
    public static T RandomElement<T>(this IEnumerable<T> source)
    {
        if (source == null || !source.Any())
            return default;

        return source.ElementAt(Random.Range(0, source.Count()));
    }
}
