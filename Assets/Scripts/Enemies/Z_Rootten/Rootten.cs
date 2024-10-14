using System.Collections;
using UnityEngine;

public class Rootten : BaseEnemy, IBurnable
{
    public CustomAudio GrabStart, GrabHit;
    private bool IsPlayerGrabbed { get; set; }
    private bool isGrabbing { get; set; }
    private IEnemyTarget GrabbedTarget { get; set; }
    private AttackTrigger GrabTrigger;
    private bool FinishedSpawning;
    private Transform GrabCenter;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rootten;
        MovementSpeed = 0.0f;
        AccelerationSpeed = 0f;
        Health = 25f;
        Damage = 22f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        GrabTrigger = transform.Find("GrabArea").GetComponent<AttackTrigger>();
        GrabTrigger.OnTagTriggered += OnTargetGrab;
        GrabCenter = transform.Find("GrabCenter");
        base.Start();

        HealthBar.AnimationSpeed = 5f;
        HealthBar.transform.localScale = new(HealthBar.transform.localScale.x * 0.6f, HealthBar.transform.localScale.y * 0.5f, HealthBar.transform.localScale.z);
    }

    public void ActiveBurningParticles(BurningEffect burnFx)
    {
        BodyFlames.StartFire(burnFx, true);
    }

    public void DeactivateFireParticles()
    {
        BodyFlames.StopFire();
    }

    protected override bool isIdle => base.isIdle && !IsPlayerGrabbed;

    protected override void Update()
    {
        if (!IsAlive || isDying)
        {
            if (DeathTime + (DeathFadeOutDelayMs / 1000) + 3 < Time.time)
            {
                DeathFadeOutDelayMs = 0;
                StartCoroutine(StartDeathFadeOutCountDown());
            }
            Animation();
            return;
        }

        var closestTarget = GetClosestTarget();
        if (closestTarget != null && IsInAttackRange)
            StartGrab(closestTarget);

        if (IsPlayerGrabbed && GrabbedTarget != null)
            StartAttack(GrabbedTarget);

        var targetDirection = Mathf.Sign(closestTarget.transform.position.x - transform.position.x);

        if (HealthBar != null)
            HealthBar.transform.position = transform.position + new Vector3(targetDirection * (SpriteRenderer.bounds.size.x * 0.1f), SpriteRenderer.bounds.size.y / 2.6f, 0);

        if (FinishedSpawning)
            FlipEnemy(targetDirection);

        Animation();
    }


    protected override void Movement(IEnemyTarget target)
    {
    }

    protected override void OnSpawnEnd()
    {
        base.OnSpawnEnd();

        FinishedSpawning = true;
        transform.Find("Dirt").SetParent(transform.parent);
    }

    private void StartGrab(IEnemyTarget target)
    {
        if (isAttacking || IsPlayerGrabbed || isGrabbing)
            return;

        if (!FinishedSpawning)
            return;

        if (target == null)
            return;

        GrabStart.PlayIfNotNull(AudioSource, AudioTypes.Enemies);

        isGrabbing = true;
    }

    public void OnGrabHit()
    {
        HitTargetsIds.Clear();
        GrabTrigger.gameObject.SetActive(true);
        StartCoroutine(DeactivateGrabTrigger(0.1f));
    }
    private IEnumerator DeactivateGrabTrigger(float duration)
    {
        yield return new WaitForSeconds(duration);
        GrabTrigger.gameObject.SetActive(false);
    }

    public void OnTargetGrab(Collider2D targetCollider)
    {
        IEnemyTarget target = targetCollider.GetComponent<IEnemyTarget>();
        if (target == null)
            return;

        int targetInstanceId = target.gameObject.GetInstanceID();
        if (HitTargetsIds.Contains(targetInstanceId))
            return;

        GrabHit.PlayIfNotNull(AudioSource, AudioTypes.Enemies);

        GrabbedTarget = target;
        IsPlayerGrabbed = true;

        AddMagnetEffect(GrabbedTarget);

        StartAttack(target);

        HitTargetsIds.Add(targetInstanceId);
    }

    public void OnGrabEnd()
    {
        HitTargetsIds.Clear();
        isGrabbing = false;
    }

    private void AddMagnetEffect(IEnemyTarget target)
    {
        if (target == null) return;

        if (target.gameObject.GetComponentInChildren<MagnetEffect>() != null)
            return;

        var magnetEffectObj = new GameObject("MagnetEffect");
        var magnetEffect = magnetEffectObj.AddComponent<MagnetEffect>();
        magnetEffect.MagnetStrength = new(500f, 200f);
        magnetEffect.MaxDistance = 2f;

        magnetEffect.SetOwner(this);
        magnetEffect.OriginPoint = GrabCenter;

        magnetEffectObj.transform.SetParent(target.transform);
        magnetEffect.SetEffect(5000, 1000, true);
    }

    private void RemoveMagnetEffect(IEnemyTarget target)
    {
        if (target == null) return;

        var magnet = target.gameObject.GetComponentInChildren<MagnetEffect>();

        if (magnet != null)
            Destroy(magnet.gameObject);
    }

    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();

        if (GrabbedTarget != null && DistanceFrom(GrabbedTarget) > DistanceFrom(GrabTrigger.transform))
        {
            RemoveMagnetEffect(GrabbedTarget);
            IsPlayerGrabbed = false;
            GrabbedTarget = null;
        }
    }

    public override void Die(string lastDamagedBodyPartName, IEnemyTarget attacker)
    {
        RemoveMagnetEffect(GrabbedTarget);
        base.Die(lastDamagedBodyPartName, attacker);

        GrabTrigger.OnTagTriggered -= OnTargetGrab;
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        Animator.SetBool("isPlayerGrabbed", IsPlayerGrabbed);

        if (isGrabbing) Animator.SetTrigger("Grab");
        else Animator.ResetTrigger("Grab");
    }

    private void OnDestroy()
    {
        RemoveMagnetEffect(GrabbedTarget);
    }
}
