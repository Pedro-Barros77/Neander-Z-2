using UnityEngine;

public class ShortBarrel : ShotgunWeapon
{
    protected override void Awake()
    {
        base.Awake();
        Type = WeaponTypes.ShortBarrel;
        IsPrimary = true;

        BulletType = BulletTypes.Shotgun;
        Damage = 3f;
        FireRate = 1.5f;
        MagazineSize = 5;
        MagazineBullets = MagazineSize;
        ShellPelletsCount = 12;
        PelletsDispersion = 50f;
        UseMagazine = false;
        BulletSpeed = 20f;
        ReloadTimeMs = 1000f;
        SwitchTimeMs = 350f;
        BulletMaxRange = 8f;
        MaxDamageRange = 3f;
        MinDamageRange = 6f;
        ShootVolume = 1f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.1f, 0f);
    }
}
