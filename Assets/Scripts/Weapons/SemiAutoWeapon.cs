using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SemiAutoWeapon : BaseWeapon
{
    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        MagazineBullets--;

        return bulletInstances;
    }
}
