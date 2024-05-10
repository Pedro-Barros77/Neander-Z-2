using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SniperWeapon : BaseWeapon
{
    /// <summary>
    /// Se o jogador est� puxando o ferrolho da arma atualmente.
    /// </summary>
    public bool IsPullingBolt { get; protected set; }
    /// <summary>
    /// N�mero de alvos que a sniper atinge antes do proj�til ser destru�do.
    /// </summary>
    public int MaxPierceCount => (Data as SniperData).MaxPierceCount;
    /// <summary>
    /// Multiplicador de dano do proj�til ao atingir um alvo.
    /// </summary>
    public float PierceDamageMultiplier => (Data as SniperData).PierceDamageMultiplier;
    /// <summary>
    /// Se est� pendente o manuseio do ferrolho (n�o pode atirar antes at� terminar).
    /// </summary>
    protected bool IsBoltActionPending;

    protected override void Start()
    {
        base.Start();
        IsPullingBolt = true;
        IsBoltActionPending = true;
    }

    public override bool CanShoot()
    {
        return base.CanShoot() && !IsPullingBolt && !IsBoltActionPending;
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        Data.MagazineBullets--;

        if (MagazineBullets > 0)
            IsBoltActionPending = true;

        return bulletInstances;
    }

    protected override List<GameObject> CreateBullets(float angleDegrees)
    {
        var bullets = base.CreateBullets(angleDegrees);
        var bullet = bullets[0].GetComponent<Projectile>();

        bullet.MaxPierceCount = MaxPierceCount;
        bullet.PierceDamageMultiplier = PierceDamageMultiplier;

        return bullets;
    }

    public override bool Reload()
    {
        if (MagazineBullets == 0)
            IsBoltActionPending = true;

        bool canReload = base.Reload();

        return canReload;
    }

    public override void OnReloadEnd()
    {
        base.OnReloadEnd();

        if (IsBoltActionPending)
            IsPullingBolt = true;

        if (MagazineBullets < MagazineSize && !isShooting)
            Reload();
    }

    public override void OnShootEnd()
    {
        base.OnShootEnd();

        if (IsBoltActionPending)
            IsPullingBolt = true;
    }

    public override void OnPumpEnd()
    {
        base.OnPumpEnd();

        IsBoltActionPending = false;
        IsPullingBolt = false;
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        if (IsPullingBolt) Animator.SetTrigger("BoltAction");
        else Animator.ResetTrigger("BoltAction");
        Animator.SetFloat("boltActionSpeed", FireRate);
    }
}
