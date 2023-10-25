using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExtensions
{
    public static float RadToDeg(this float input)
    {
        float deg = (input * Mathf.Rad2Deg) % 360;

        return deg < 0 ? deg + 360 : deg;
    }

    public static bool GreaterOrAproxEqual(this float input, float value) =>
        input >= value || Mathf.Approximately(input, value);

    public static bool LessOrAproxEqual(this float input, float value) =>
        input <= value || Mathf.Approximately(input, value);
}
