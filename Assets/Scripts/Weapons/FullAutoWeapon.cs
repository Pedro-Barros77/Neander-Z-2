using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FullAutoWeapon : BaseWeapon
{
    public override bool CanShoot()
    {
        if (MagazineBullets <= 0 && !Constants.GetActionDown(InputActions.Shoot))
            return false;

        return base.CanShoot();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        Data.MagazineBullets--;

        return bulletInstances;
    }
}
