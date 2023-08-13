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
    }

    public override GameObject Shoot()
    {
        var bulletInstance = base.Shoot();

        var bullet = bulletInstance.GetComponent<Projectile>();
        bullet.MaxDamageRange = MaxDamageRange;
        bullet.MinDamageRange = MinDamageRange;

        return bulletInstance;
    }
}
