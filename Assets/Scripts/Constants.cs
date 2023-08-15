using System.Collections.Generic;

public static class Constants
{
    /// <summary>
    /// Definição de quais armas são apenas primárias e quais podem ser de ambos os tipos. Retorna true se a arma for apenas primária.
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
