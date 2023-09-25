using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class Constants
{
    /// <summary>
    /// Define se a arma é primária ou secundária.
    /// </summary>
    /// <param name="weaponType">O tipo da arma.</param>
    /// <returns>Se a arma é primária, caso contrário, secundária.</returns>
    public static bool IsPrimary(WeaponTypes weaponType)
    {
        return weaponType switch
        {
            WeaponTypes.Machete => false,
            WeaponTypes.Colt_1911 => false,
            WeaponTypes.Deagle => false,
            WeaponTypes.Beretta_93R => false,

            WeaponTypes.ShortBarrel => true,
            WeaponTypes.UZI => true,
            WeaponTypes.RPG => true,
            WeaponTypes.SV98 => true,
            WeaponTypes.M16 => true,
            WeaponTypes.Scar => true,
            WeaponTypes.ScarDebug => true,

            _ => false,
        };
    }

    /// <summary>
    /// Calcula uma cor baseada em um valor e um máximo, com uma porcentagem de breakpoint.
    /// </summary>
    /// <param name="value">O valor real.</param>
    /// <param name="max">O valor máximo.</param>
    /// <param name="percentageBreakpoint">A porcentagem de breakpoint (onde a cor deixa de ser a full e passa a ser warning).</param>
    /// <param name="fullColor">A cor quando o valor está acima do breakpoint.</param>
    /// <param name="warningColor">A cor quando o valor está abaixo do breakpoint.</param>
    /// <param name="zeroColor">A cor quando o valor for zero.</param>
    /// <returns></returns>
    public static Color32 GetAlertColor(int value, int max, float percentageBreakpoint, Color32? fullColor = null, Color32? warningColor = null, Color32? zeroColor = null)
    {
        if (value == 0)
            return zeroColor ?? Colors.RedMoney;

        var percentage = (float)value / max;

        if (percentage > percentageBreakpoint)
            return fullColor ?? Color.white;

        return warningColor ?? Colors.YellowAmmo;
    }

    public readonly struct Colors
    {
        public static readonly Color32 GreenMoney = new(72, 164, 80, 255);
        public static readonly Color32 RedMoney = new(205, 86, 99, 255);
        public static readonly Color32 YellowAmmo = new(230, 220, 80, 255);
    }
}
