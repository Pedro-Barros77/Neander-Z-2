using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Neander Z/Store/Weapon Data", order = 1)]
public class StoreWeaponData : StoreItemData
{
    public WeaponTypes WeaponType;
    public bool IsPrimary;
    public BulletTypes BulletType;
    public float HeadshotMultiplier;
    public int MagazineBullets;
    public int PelletsCount;
    public float Dispersion;

    public StoreWeaponData() : base(true)
    {
    }
}
