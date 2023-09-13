public static class Constants
{
    /// <summary>
    /// Define se a arma � prim�ria ou secund�ria.
    /// </summary>
    /// <param name="weaponType">O tipo da arma.</param>
    /// <returns>Se a arma � prim�ria, caso contr�rio, secund�ria.</returns>
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
