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

    /// <summary>
    /// Faz o inverso de Color.Lerp, calculando o valor de interpolação de uma cor em relação a um intervalo de cores.
    /// </summary>
    /// <param name="source">A cor fonte, de onde será tirado o valor da interpolação.</param>
    /// <param name="startColor">A cor inicial do range de cores.</param>
    /// <param name="endColor">A cor final do range de cores.</param>
    /// <returns>O valor da interpolação da cor source entre as cores Start e End.</returns>
    public static float GetRatioFromRange(this Color32 source, Color32 startColor, Color32 endColor)
    {
        // Calculate the distances between colors Source, start, and end
        float distanceSourceStart = ColorDistance(source, startColor);
        float distanceSourceEnd = ColorDistance(source, endColor);
        float distanceStartEnd = ColorDistance(startColor, endColor);

        // Calculate the proportion of the distance between Source and Start relative to the total distance between Start and End
        float interpolationValue = distanceSourceStart / distanceStartEnd;

        // If Source is closer to color End than to color Start, adjust the interpolation value accordingly
        if (distanceSourceEnd < distanceSourceStart)
        {
            interpolationValue = 1f - distanceSourceEnd / distanceStartEnd;
        }

        return interpolationValue;
    }

    /// <summary>
    /// Calcula a distância entre duas cores.
    /// </summary>
    /// <param name="color1">A primeira cor.</param>
    /// <param name="color2">A segunda cor.</param>
    /// <returns>O valor representando a distância entre as cores.</returns>
    private static float ColorDistance(Color color1, Color color2)
    {
        return Mathf.Sqrt(Mathf.Pow(color1.r - color2.r, 2) +
                          Mathf.Pow(color1.g - color2.g, 2) +
                          Mathf.Pow(color1.b - color2.b, 2));
    }
}
