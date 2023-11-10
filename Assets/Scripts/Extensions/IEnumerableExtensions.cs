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
}
