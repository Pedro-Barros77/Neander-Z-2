using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    /// <summary>
    /// Converte uma cor para hexadecimal.
    /// </summary>
    /// <param name="color">A cor a ser convertida.</param>
    /// <returns>O valor hexadecimal da cor.</returns>
    public static string ToHex(this Color32 color)
    {
        return '#' + ColorUtility.ToHtmlStringRGB(color);
    }

    /// <summary>
    /// Converte uma string hexadecimal para cor.
    /// </summary>
    /// <param name="hex">Uma string contendo uma cor em hexadecimal.</param>
    /// <returns>A cor convertida ou branco caso a conversão falhe.</returns>
    public static Color32 FromHexToColor(this string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        return Color.white;
    }
}
