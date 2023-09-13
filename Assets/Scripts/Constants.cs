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

            _ => false,
        };
    }
}
