using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SemiAutoWeapon : BaseWeapon
{
    protected override void Update()
    {
        Animator.SetFloat("shootSpeed", FireRate / 10);

        if (MenuController.Instance.IsGamePaused)
            return;

        base.Update();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        Animator.SetTrigger("Shoot");

        var bullet = bulletInstances.First().GetComponent<Projectile>();
        bullet.HasGravity = false;

        MagazineBullets--;

        return bulletInstances;
    }

    public override bool Reload()
    {
        bool canReload = base.Reload();
        if (canReload)
            Animator.SetTrigger("Reload");

        return canReload;
    }
}
