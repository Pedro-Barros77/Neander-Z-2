using System.Collections.Generic;
using UnityEngine;

public class M79GrenadeLauncher : LauncherWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, -0.25f, 0f);
    }

    protected override List<GameObject> CreateBullets(float angleDegrees)
    {
        var bulletInstance = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.Euler(0f, 0f, angleDegrees), BulletsContainer);
        var bullet = bulletInstance.GetComponent<GrenadeBullet>();

        bullet.Type = BulletType;
        bullet.StartPos = BulletSpawnPoint.position;
        bullet.AngleDegrees = PlayerWeaponController.AimAngleDegrees;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.MinDamage = MinDamage;
        bullet.MaxDistance = BulletMaxRange;
        bullet.MaxDamageRange = MaxDamageRange;
        bullet.MinDamageRange = MinDamageRange;
        bullet.PlayerOwner = Player;
        bullet.HeadshotMultiplier = HeadshotMultiplier;
        bullet.OnBulletKill += OnBulletKill;
        bullet.ShotTime = Time.time;

        var data = Data as LauncherData;
        bullet.ExplosionMinDamageRadius = data.ExplosionMinDamageRadius;
        bullet.ExplosionMaxDamageRadius = data.ExplosionMaxDamageRadius;
        bullet.ExplosionSpriteSize = data.ExplosionSpriteSize;
        bullet.FlyInfinitely = data.FlyInfinitely;
        bullet.GravityScale = data.GravityScale;
        bullet.Init();

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);

        return new List<GameObject>() { bulletInstance };
    }
}
