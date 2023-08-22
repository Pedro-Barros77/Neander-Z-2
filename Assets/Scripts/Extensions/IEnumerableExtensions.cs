using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static string JoinString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }
}
