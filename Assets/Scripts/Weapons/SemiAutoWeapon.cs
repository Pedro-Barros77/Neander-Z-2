using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SemiAutoWeapon : BaseWeapon
{
    protected override void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        base.Update();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        var bullet = bulletInstances.First().GetComponent<Projectile>();
        bullet.HasGravity = false;

        MagazineBullets--;

        return bulletInstances;
    }
}
