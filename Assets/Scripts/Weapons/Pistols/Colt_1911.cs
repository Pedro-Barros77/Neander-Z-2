using UnityEngine;

public class Colt_1911 : SemiAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        Type = WeaponTypes.Colt_1911;
        IsPrimary = false;

        BulletType = BulletTypes.Pistol;
        Damage = 6f;
        FireRate = 4f;
        MagazineSize = 7;
        MagazineBullets = MagazineSize;
        BulletSpeed = 25f;
        ReloadTimeMs = 1000f;
        SwitchTimeMs = 200f;
        BulletMaxRange = 15f;
        MaxDamageRange = 5f;
        MinDamageRange = 10f;
        ShootVolume = 0.5f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.4f, 0f);
    }
}
