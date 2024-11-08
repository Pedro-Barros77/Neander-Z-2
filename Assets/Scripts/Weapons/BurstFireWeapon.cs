using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BurstFireWeapon : BaseWeapon
{
    /// <summary>
    /// Quantidade de disparos realizados em uma rajada dessa arma.
    /// </summary>
    public int BurstShotsCount => (Data as BurstFireData).BurstShotsCount;
    public float BurstFireRate => (Data as BurstFireData).BurstFireRate;
    public bool IsFiringBurst { get; protected set; }

    int CurrentBurstShotCount = 1;
    public override IEnumerable<GameObject> Shoot()
    {
        if (MagazineBullets <= 0)
            IsFiringBurst = false;

        var bulletInstances = base.Shoot();
        if (!bulletInstances.Any())
            return bulletInstances;

        Data.MagazineBullets--;
        lastShotTime = null;

        if (CurrentBurstShotCount < BurstShotsCount)
            StartCoroutine(FireNextBurstShot());
        else
        {
            IsFiringBurst = false;
            CurrentBurstShotCount = 1;
            lastShotTime = Time.time;
        }

        return bulletInstances;
    }

    protected virtual IEnumerator FireNextBurstShot()
    {
        IsFiringBurst = true;
        yield return new WaitForSeconds((FIRE_RATE_RATIO / BurstFireRate) / 1000);
        CurrentBurstShotCount++;
        Shoot();
    }

    public override bool Reload()
    {
        if (IsFiringBurst)
            return false;

        bool reloading = base.Reload();
        if (reloading)
        {
            IsFiringBurst = false;
            CurrentBurstShotCount = 1;
        }
        return reloading;
    }

    public override bool BeforeSwitchWeapon()
    {
        if (IsFiringBurst)
            return false;

        return base.BeforeSwitchWeapon();
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        Animator.SetFloat("shootSpeed", FireRate / 3);
    }
}
