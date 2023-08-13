using UnityEngine;

public class SemiAutoWeapon : BaseWeapon
{
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();
    }

    public override GameObject Shoot()
    {
        var bulletInstance = base.Shoot();

        var bullet = bulletInstance.GetComponent<Projectile>();
        bullet.HasGravity = false;

        return bulletInstance;
    }
}
