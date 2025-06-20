using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShotgunWeapon : BaseWeapon
{
    /// <summary>
    /// Quantidade de ballins disparados do cartucho da escopeta.
    /// </summary>
    public int ShellPelletsCount => (Data as ShotgunData).ShellPelletsCount;
    /// <summary>
    /// A disper��o dos ballins disparados.
    /// </summary>
    public float PelletsDispersion => (Data as ShotgunData).PelletsDispersion;
    /// <summary>
    /// Se a arma est� sendo bombeada atualmente.
    /// </summary>
    public bool IsPumping { get; protected set; }
    /// <summary>
    /// Se a arma utiliza cartucho/pente para o carregamento.
    /// </summary>
    public bool UseMagazine => (Data as ShotgunData).UseMagazine;
    public int MaxPierceCount => (Data as ShotgunData).MaxPierceCount;
    public float PierceDamageMultiplier => (Data as ShotgunData).PierceDamageMultiplier;
    /// <summary>
    /// Se est� pendente o bombeamento da arma (n�o pode atirar antes at� bombear).
    /// </summary>
    private bool IsPumpPending;
    /// <summary>
    /// Se o carregamento em cadeia foi cancelado.
    /// </summary>
    private bool ReloadCanceled;
    /// <summary>
    /// Lista de tempos em que cada disparo foi efetuado e sua precis�o contabilizada (para previnir que cada balin da escopeta contabilize na precis�o do disparo).
    /// </summary>
    private List<float> HandledShotScoreTimes = new();

    protected override void Start()
    {
        base.Start();
        if (FireMode == FireModes.PumpAction)
        {
            IsPumping = true;
            IsPumpPending = true;
        }
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

        Data.MagazineBullets--;

        if (MagazineBullets > 0 && FireMode == FireModes.PumpAction)
            IsPumpPending = true;

        return bulletInstances;
    }

    protected override List<GameObject> CreateBullets(float radiansAngle)
    {
        List<GameObject> bulletsInstances = new();

        float shotTime = Time.time;
        void InitBullet(Projectile bullet)
        {
            bullet.Type = BulletTypes.Pistol;
            bullet.StartPos = BulletSpawnPoint.position;
            bullet.Speed = BulletSpeed;
            bullet.Damage = Damage;
            bullet.MaxDistance = BulletMaxRange;
            bullet.MaxDamageRange = MaxDamageRange;
            bullet.MinDamageRange = MinDamageRange;
            bullet.PlayerOwner = Player;
            bullet.HeadshotMultiplier = HeadshotMultiplier;
            bullet.OnBulletKill += OnBulletKill;
            bullet.ShotTime = shotTime;
            bullet.MaxPierceCount = MaxPierceCount;
            bullet.PierceDamageMultiplier = PierceDamageMultiplier;
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

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);

        return bulletsInstances;
    }

    public override bool Reload()
    {
        if (MagazineBullets == 0 && FireMode == FireModes.PumpAction)
            IsPumpPending = true;

        if (!UseMagazine)
            ReloadCanceled = false;

        bool canReload = base.Reload();

        return canReload;
    }

    public override bool BeforeSwitchWeapon()
    {
        if (!UseMagazine)
            ReloadCanceled = true;
        return base.BeforeSwitchWeapon();
    }

    public override void OnReloadedChamber()
    {
        if (UseMagazine)
        {
            base.OnReloadedChamber();
            return;
        }

        Data.MagazineBullets += 1;
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

        if (FireMode == FireModes.PumpAction)
        {
            Animator.SetFloat("pumpSpeed", FireRate);
            if (IsPumping) Animator.SetTrigger("Pump");
            else Animator.ResetTrigger("Pump");
        }
    }

    protected override void OnBulletKill(Projectile projectile, IPlayerTarget playerTarget, IEnemyTarget enemyTarget)
    {
        if (playerTarget != null)
        {
            if (HandledShotScoreTimes.Contains(projectile.ShotTime))
                return;

            HandledShotScoreTimes.Add(projectile.ShotTime);
            WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
        }
    }
}
