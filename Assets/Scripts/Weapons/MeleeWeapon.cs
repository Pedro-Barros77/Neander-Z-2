using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
    protected int AttackAnimationsCount => (Data as MeleeData).AttackAnimationsCount;
    protected float AttackStaminaCost => (Data as MeleeData).AttackStaminaCost;

    [SerializeField]
    protected List<CustomAudio> HitSounds;
    protected AttackTrigger AttackTrigger;

    protected List<int> HitTargetsIds = new();
    protected int RandomAttackAnimationIndex = 1;
    protected bool AddedTargetHitScore;

    protected override void Awake()
    {
        AttackTrigger = transform.GetComponentInChildren<AttackTrigger>(true);
        if (AttackTrigger != null)
            AttackTrigger.OnTagTriggered += OnTargetHit;

        base.Awake();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var emptyBullets = Enumerable.Empty<GameObject>();

        if (!CanShoot())
            return emptyBullets;

        Player.LoseStamina(AttackStaminaCost + Player.Data.AttackStaminaDrain);

        AddedTargetHitScore = false;
        HitTargetsIds.Clear();

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);
        isShooting = true;
        lastShotTime = Time.time;

        RandomAttackAnimationIndex = Random.Range(1, AttackAnimationsCount + 1);

        ShootSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);

        return emptyBullets;
    }

    public override bool CanShoot()
    {
        if (IsSwitchingWeapon)
            return false;

        if (Player.Stamina < AttackStaminaCost)
            return false;

        var now = Time.time;

        var delayMs = FIRE_RATE_RATIO / FireRate;
        var diff = now - (delayMs / 1000);

        if (lastShotTime != null && diff <= lastShotTime)
            return false;

        return true;
    }

    public override bool BeforeSwitchWeapon()
    {
        isShooting = false;
        IsSwitchingWeapon = true;

        return true;
    }

    protected override void SyncAnimationStates()
    {
        Animator.SetFloat("shootSpeed", FireRate / 5);

        if (isShooting) Animator.SetTrigger($"Attack_0{RandomAttackAnimationIndex}");
        else Animator.ResetTrigger($"Attack_0{RandomAttackAnimationIndex}");
    }

    /// <summary>
    /// Fun��o chamada ao atingir um alvo com o ataque.
    /// </summary>
    /// <param name="targetCollider">O collider do alvo atacado.</param>
    protected virtual void OnTargetHit(Collider2D targetCollider)
    {
        IPlayerTarget target = targetCollider.GetComponentInParent<IPlayerTarget>();
        if (target == null)
            return;

        int targetInstanceId = target.gameObject.GetInstanceID();

        if (HitTargetsIds.Contains(targetInstanceId))
            return;

        HitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);

        Vector2 hitPosition = targetCollider.ClosestPoint(AttackTrigger.transform.position);

        var damageProps = new TakeDamageProps(DamageTypes.Cutting, Damage, Player, HeadshotMultiplier)
            .WithBodyPart(targetCollider.name)
            .WithHitPosition(hitPosition)
            .WithHitEffectDirection(-transform.right);

        target.TakeDamage(damageProps);

        if (target.IsAlive)
        {
            if (!AddedTargetHitScore)
                WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
            AddedTargetHitScore = true;
        }

        HitTargetsIds.Add(targetInstanceId);
    }

    public override bool NeedsReload()
    {
        return false;
    }
}
