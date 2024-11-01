using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Claymore : BaseThrowable
{
    [SerializeField]
    public GameObject ExplosionPrefab;

    [SerializeField]
    public Collider2D ExplosionCollider, DetectionCollider;

    [SerializeField]
    public CustomAudio ActivationSound;

    private Transform ExplosionSpawnPoint;
    readonly string[] IgnoreBodyPartsNames = { "Plate" };
    readonly float ExplosionPushForce = 2600;
    private LineRenderer Laser1;
    private LineRenderer Laser2;
    private float ArmedDirection;

    protected override void Awake()
    {
        Type = ThrowableTypes.Claymore;
        hitSoundIntervalMs = 5000f;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        ExplosionSpawnPoint = Sprite.transform.Find("ExplosionSpawnPoint");
        Laser1 = Sprite.transform.Find("Laser1").GetComponent<LineRenderer>();
        Laser2 = Sprite.transform.Find("Laser2").GetComponent<LineRenderer>();

        TargetLayerMask += LayerMask.GetMask("Player");
    }

    public override void Throw()
    {
        base.Throw();

        IsActivated = false;

        ArmedDirection = PlayerWeaponController.IsAimingLeft ? -1 : 1;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = transform.localScale.WithX(ArmedDirection * transform.localScale.x);
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        StartCoroutine(ActivateDelayed());
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsActivated)
            return;

        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        var target = collision.gameObject.GetComponentInParent<IPlayerTarget>();

        if (target == null)
            return;

        if (!target.IsAlive)
            return;

        Detonate();
    }

    protected override void Detonate()
    {
        if(HasDetonated)
            return;
        DetonateSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
        HasDetonated = true;
        Laser1.enabled = false;
        Laser2.enabled = false;

        StartCoroutine(ExplodeDelayed());
    }

    private IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(0.4f);

        Explode();
    }

    private IEnumerator ActivateDelayed()
    {
        yield return new WaitForSeconds(1f);

        IsActivated = true;
        DetectionCollider.gameObject.SetActive(true);
        Laser1.enabled = true;
        Laser2.enabled = true;
        ActivationSound.PlayIfNotNull(AudioSource, AudioTypes.Player);
    }

    private void Explode()
    {
        var hitPosition = ExplosionSpawnPoint.position;

        var explosion = Instantiate(ExplosionPrefab, hitPosition, Quaternion.identity, EffectsContainer);
        var explosionScale = Vector3.one * Data.EffectSpriteSize;
        explosionScale = explosionScale.WithX(ArmedDirection * explosionScale.x);
        explosion.transform.localScale = explosionScale;


        List<Collider2D> results = new();
        Physics2D.OverlapCollider(ExplosionCollider, results);

        var enemiesHit = results.Select(x => new { target = x.GetComponentInParent<IPlayerTarget>(), collider = x }).Where(x => x.target != null && x.collider.gameObject.CompareTag("Enemy")).ToList();
        var playersHit = results.Select(x => new { target = x.GetComponentInParent<IEnemyTarget>(), collider = x }).Where(x => x.target != null && x.collider.gameObject.CompareTag("Player")).ToList();

        foreach (var hit in enemiesHit)
        {
            IPlayerTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var enemyHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(enemyHitPoint, transform.position);

            if (distance > EffectMinRange)
                continue;

            if (!target.IsAlive)
                continue;

            SetTargetHit();
            int targetId = target.transform.GetInstanceID();

            if (PiercedTargetsIds.Contains(targetId))
                continue;

            PiercedTargetsIds.Add(targetId);

            if (target is IKnockBackable knockBackable)
            {
                Vector3 direction = target.transform.position - transform.position;
                knockBackable.TakeKnockBack(ExplosionPushForce, direction.normalized);
            }

            var clampedDistance = Mathf.Clamp(distance, EffectMaxRange, EffectMinRange);
            var percentage = (clampedDistance - EffectMaxRange) / (EffectMinRange - EffectMaxRange);

            float damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

            var damageProps = new TakeDamageProps(DamageTypes.Explosion, damage, PlayerOwner, HeadshotMultiplier)
                .WithBodyPart(IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name)
                .WithHitPosition(enemyHitPoint)
                .WithHitEffectDirection(-transform.right);

            target.TakeDamage(damageProps);
        }

        foreach (var hit in playersHit)
        {
            IEnemyTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var playerHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(playerHitPoint, transform.position);

            if (distance > EffectMinRange)
                continue;

            if (!target.IsAlive)
                continue;

            SetTargetHit();
            int targetId = target.gameObject.GetInstanceID();

            if (PiercedTargetsIds.Contains(targetId))
                continue;

            PiercedTargetsIds.Add(targetId);

            if (target is IKnockBackable knockBackable)
            {
                Vector3 direction = target.transform.position - transform.position;
                knockBackable.TakeKnockBack(ExplosionPushForce, direction.normalized);
            }

            var clampedDistance = Mathf.Clamp(distance, EffectMaxRange, EffectMinRange);
            var percentage = (clampedDistance - EffectMaxRange) / (EffectMinRange - EffectMaxRange);

            float damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

            var damageProps = new TakeDamageProps(DamageTypes.Explosion, damage, PlayerOwner, HeadshotMultiplier)
                .WithBodyPart(IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name)
                .WithHitPosition(playerHitPoint)
                .WithHitEffectDirection(-transform.right);

            target.TakeDamage(damageProps);
        }

        Sprite.enabled = false;
        Collider.enabled = false;
        Laser1.enabled = false;
        Laser2.enabled = false;

        StartCoroutine(KillSelfDelayed(2));
    }
}
