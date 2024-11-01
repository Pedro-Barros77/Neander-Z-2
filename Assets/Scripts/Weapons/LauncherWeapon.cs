using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LauncherWeapon : BaseWeapon
{
    public float ExplosionMaxDamageRadius => (Data as LauncherData).ExplosionMaxDamageRadius;
    public float ExplosionMinDamageRadius => (Data as LauncherData).ExplosionMinDamageRadius;

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        Data.MagazineBullets--;

        return bulletInstances;
    }

    protected override List<GameObject> CreateBullets(float angleDegrees)
    {
        var bullets = base.CreateBullets(angleDegrees);
        if(!bullets.Any())
            return bullets;

        var bullet = bullets[0].GetComponent<RocketBullet>();
        var data = Data as LauncherData;
        bullet.ExplosionMinDamageRadius = data.ExplosionMinDamageRadius;
        bullet.ExplosionMaxDamageRadius = data.ExplosionMaxDamageRadius;
        bullet.ExplosionSpriteSize = data.ExplosionSpriteSize;

        return bullets;
    }
}
