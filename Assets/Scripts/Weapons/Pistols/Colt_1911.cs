using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Colt_1911 : SemiAutoWeapon
{
    protected override void Awake()
    {
        Type = WeaponTypes.Colt_1911;
        base.Awake();

        BulletType = BulletTypes.Pistol;
        Damage = 6f;
        FireRate = 4f;
        MagazineSize = 7;
        MagazineBullets = MagazineSize;
        BulletSpeed = 25f;
        ReloadTimeMs = 1000f;
        BulletMaxRange = 15f;
        MaxDamageRange = 5f;
        MinDamageRange = 10f;
        ShootVolume = 0.3f;
    }
}
