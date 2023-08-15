using System.Collections.Generic;

public static class Constants
{
    /// <summary>
    /// Defini��o de quais armas s�o apenas prim�rias e quais podem ser de ambos os tipos. Retorna true se a arma for apenas prim�ria.
    /// </summary>
    public static readonly Dictionary<WeaponTypes, bool> IsPrimaryWeapon = new()
    {
        { WeaponTypes.Colt_1911, false },
        { WeaponTypes.Deagle, false },
        { WeaponTypes.Beretta_93R, false },
        { WeaponTypes.ShortBarrel, true },
        { WeaponTypes.UZI, true },
        { WeaponTypes.Machete, false },
        { WeaponTypes.RPG, true },
        { WeaponTypes.SV98, true },
        { WeaponTypes.M16, true },
        { WeaponTypes.Scar, true },
    };
}
