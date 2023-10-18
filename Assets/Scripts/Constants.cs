using System.Linq;
using UnityEngine;

public static class Constants
{
    public static bool EnableDevKeybinds { get; private set; } = true;
    public static float MaxWeaponDamage { get; private set; } = 60;
    public static float MaxWeaponFireRate { get; private set; } = 18;
    public static float MaxWeaponReloadSpeed { get; private set; } = 12;
    public static float MaxWeaponRange { get; private set; } = 48;

    static float ReloadSpeedRatio = 5000;

    public static bool GetActionDown(InputActions action) => action == InputActions.SwitchWeapon ? Input.mouseScrollDelta.y != 0 
                                                       : GetActionPerformed(action, 0);
    public static bool GetAction(InputActions action) => GetActionPerformed(action, 1);
    public static bool GetActionUp(InputActions action) => GetActionPerformed(action, 2);

    private static bool GetActionPerformed(InputActions action, int pressingState) =>
        MenuController.Instance.Keybind.Inputs.Any(inp => inp.Any(keybind => keybind.Action == action
            && (
                       (pressingState == 0 && Input.GetKeyDown(keybind.Key))
                    || (pressingState == 1 && Input.GetKey(keybind.Key))
                    || (pressingState == 2 && Input.GetKeyUp(keybind.Key))
               )));


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
    /// Calcula o dano dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>O Dano calculado.</returns>
    public static float CalculateDamage(BaseWeaponData weaponData)
    {
        float damage = weaponData.Damage;
        if (weaponData is ShotgunData shotgunData)
            damage = shotgunData.Damage * shotgunData.ShellPelletsCount;

        return damage;
    }

    /// <summary>
    /// Calcula a cadência de tiro dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>A cadência de tiro calculada.</returns>
    public static float CalculateFireRate(BaseWeaponData weaponData)
    {
        float fireRate = weaponData.FireRate;

        switch (weaponData.FireMode)
        {
            case FireModes.Burst:
                if (weaponData is BurstFireData burstData)
                    fireRate = (burstData.FireRate + burstData.BurstFireRate) / 2;
                break;
        }

        return fireRate;
    }

    /// <summary>
    /// Calcula a velocidade de recarga dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>A velocidade de recarga calculada.</returns>
    public static float CalculateReloadSpeed(BaseWeaponData weaponData)
    {
        float reloadSpeed = ReloadSpeedRatio / weaponData.ReloadTimeMs;

        switch (weaponData.ReloadType)
        {
            case ReloadTypes.NoReload:
                reloadSpeed = 0;
                break;
            case ReloadTypes.SingleBullet:
                reloadSpeed = ReloadSpeedRatio / (weaponData.ReloadTimeMs * weaponData.MagazineSize);
                break;
        }

        return reloadSpeed;
    }

    /// <summary>
    /// Calcula o alcance dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>O alcance calculado.</returns>
    public static float CalculateRange(BaseWeaponData weaponData)
    {
        float range = (weaponData.MinDamageRange + weaponData.MaxDamageRange + weaponData.BulletMaxRange) / 3;
        if (weaponData is MeleeData || weaponData.BulletType == BulletTypes.Melee || weaponData.FireMode == FireModes.Melee)
            range = 0;

        return range;
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
