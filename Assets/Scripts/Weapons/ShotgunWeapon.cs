using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShotgunWeapon : BaseWeapon
{
    /// <summary>
    /// Quantidade de ballins disparados do cartucho da escopeta.
    /// </summary>
    public int ShellPelletsCount { get; protected set; } = 12;
    /// <summary>
    /// A disperção dos ballins disparados.
    /// </summary>
    public float PelletsDispersion { get; set; } = 50f;
    /// <summary>
    /// Se a arma está sendo bombeada atualmente.
    /// </summary>
    public bool IsPumping { get; protected set; }
    /// <summary>
    /// Se a arma utiliza cartucho/pente para o carregamento.
    /// </summary>
    public bool UseMagazine { get; protected set; }
    /// <summary>
    /// Se está pendente o bombeamento da arma (não pode atirar antes até bombear).
    /// </summary>
    private bool IsPumpPending;
    /// <summary>
    /// Se o carregamento em cadeia foi cancelado.
    /// </summary>
    private bool ReloadCanceled;

    protected override void Start()
    {
        base.Start();
        IsPumping = true;
        IsPumpPending = true;
    }


    protected override void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        base.Update();
    }

    public override bool CanShoot()
    {
        return base.CanShoot() && !IsPumping && !IsPumpPending;
    }

    public override IEnumerable<GameObject> Shoot()
    {
        if (!UseMagazine && IsReloading)
        {
            ReloadCanceled = true;
            return Enumerable.Empty<GameObject>();
        }

        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        MagazineBullets--;

        if (MagazineBullets > 0)
            IsPumpPending = true;

        return bulletInstances;
    }

    protected override List<GameObject> CreateBullets(float radiansAngle)
    {
        List<GameObject> bulletsInstances = new List<GameObject>();

        void InitBullet(Projectile bullet)
        {
            bullet.Type = BulletTypes.Pistol;
            bullet.StartPos = BulletSpawnPoint.position;
            bullet.Speed = BulletSpeed;
            bullet.Damage = Damage;
            bullet.MaxDistance = BulletMaxRange;
            bullet.MaxDamageRange = MaxDamageRange;
            bullet.MinDamageRange = MinDamageRange;
            bullet.Init();
        }

        for (int i = 0; i < ShellPelletsCount; i++)
        {
            var randomAngle = PlayerWeaponController.AimAngleDegrees + Random.Range(-PelletsDispersion, PelletsDispersion);
            var bulletInstance = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.Euler(0f, 0f, randomAngle), BulletsContainer);
            var bullet = bulletInstance.GetComponent<Projectile>();
            bullet.AngleDegrees = randomAngle;
            InitBullet(bullet);
            bulletsInstances.Add(bulletInstance);
        }

        return bulletsInstances;
    }

    public override bool Reload()
    {
        if (MagazineBullets == 0)
            IsPumpPending = true;

        ReloadCanceled = false;

        bool canReload = base.Reload();

        return canReload;
    }

    public override void BeforeSwitchWeapon()
    {
        ReloadCanceled = true;
        base.BeforeSwitchWeapon();
    }

    public override void OnReloadedChamber()
    {
        if (UseMagazine)
        {
            base.OnReloadedChamber();
            return;
        }

        MagazineBullets += 1;
        Player.Backpack.SetAmmo(BulletType, Player.Backpack.GetAmmo(BulletType) - 1);
    }
    public override void OnReloadEnd()
    {
        base.OnReloadEnd();

        if (IsPumpPending)
            IsPumping = true;

        if (MagazineBullets < MagazineSize && !isShooting && !ReloadCanceled)
            Reload();
    }

    public override void OnShootEnd()
    {
        base.OnShootEnd();

        if (IsPumpPending)
            IsPumping = true;
    }

    public override void OnPumpEnd()
    {
        base.OnPumpEnd();

        IsPumpPending = false;
        IsPumping = false;
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        if (IsPumping) Animator.SetTrigger("Pump");
        else Animator.ResetTrigger("Pump");
    }
}
