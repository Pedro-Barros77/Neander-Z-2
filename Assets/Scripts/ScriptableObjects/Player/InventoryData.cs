using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Data", menuName = "Neander Z/Player/Inventory Data", order = 3)]
public class InventoryData : ScriptableObject
{
    public int BackpackLevel;

    public int MaxPistolAmmo;
    public int MaxShotgunAmmo;
    public int MaxRifleAmmo;
    public int MaxSniperAmmo;
    public int MaxRocketAmmo;

    public int PistolAmmo;
    public int ShotgunAmmo;
    public int RifleAmmo;
    public int SniperAmmo;
    public int RocketAmmo;

    public WeaponTypes EquippedPrimaryType;
    public WeaponTypes EquippedSecondaryType;

    public int CurrentWeaponIndex;
}
