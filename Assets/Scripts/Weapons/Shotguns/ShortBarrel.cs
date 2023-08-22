using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortBarrel : ShotgunWeapon
{
    protected override void Awake()
    {
        Type = WeaponTypes.ShortBarrel;
        IsPrimary = true;
        base.Awake();

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
        BulletMaxRange = 8f;
        MaxDamageRange = 3f;
        MinDamageRange = 6f;
        ShootVolume = 1f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new UnityEngine.Vector3(0f, 0.4f, 0f);
    }
}
