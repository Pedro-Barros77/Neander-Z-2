using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Colt_1911 : SemiAutoWeapon
{
    public Colt_1911()
    {
        IsPrimary = false;
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

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        var bullet = bulletInstances.First().GetComponent<Projectile>();
        bullet.MaxDamageRange = MaxDamageRange;
        bullet.MinDamageRange = MinDamageRange;

        return bulletInstances;
    }
}
