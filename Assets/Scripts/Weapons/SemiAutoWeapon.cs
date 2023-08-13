using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SemiAutoWeapon : BaseWeapon
{
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if(!bulletInstances.Any())
            return bulletInstances;

        var bullet = bulletInstances.First().GetComponent<Projectile>();
        bullet.HasGravity = false;

        return bulletInstances;
    }
}
