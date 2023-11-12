using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RocketBullet : Projectile
{
    public float ExplosionKnockbackForce { get; set; }
    public float ExplosionMaxDamageRadius { get; set; }
    public float ExplosionMinDamageRadius { get; set; }
    public float ExplosionSpriteSize { get; set; }
    public bool Exploded { get; private set; }

    [SerializeField]
    public GameObject ExplosionPrefab;

    float projectileExplodeDelay = 0.3f;
    float? projectileHitTime = null;
    float projectilePushForce = 1600, explosionPushForce = 2500;
    Quaternion startRotation;
    List<int> hitTargetsIds = new();

    readonly string[] IgnoreBodyPartsNames = { "Plate" };

    protected override void Start()
    {
        base.Start();
        StartForwardMovement();
        RotateTowardsDirection = true;
        startRotation = transform.rotation;
        TargetLayerMask += LayerMask.GetMask("Player");
    }

    protected override void Update()
    {
        base.Update();

        transform.rotation = startRotation;
        StartForwardMovement();

        if (projectileHitTime != null && Time.time > projectileHitTime + projectileExplodeDelay && !Exploded)
            Explode();
    }

    protected override void OnEnemyHit(Collider2D collision)
    {
        if (Exploded)
            return;

        var target = collision.GetComponentInParent<IPlayerTarget>();

        if (target != null)
        {
            if (target.IsAlive)
            {
                projectileHitTime ??= Time.time;

                if (target is IKnockBackable knockBackable)
                {
                    bool isGoingLeft = AngleDegrees > 90 && AngleDegrees < 270;
                    Vector3 direction = new(isGoingLeft ? -1 : 1, 0);
                    knockBackable.TakeKnockBack(projectilePushForce, direction);
                }
                else
                    projectileExplodeDelay = 0;

                IsTargetHit = true;
                LastEnemyHit = target;
            }

            var hitPosition = collision.ClosestPoint(transform.position);
            target.TakeDamage(Damage / 3, HeadshotMultiplier, collision.name, PlayerOwner);
            target.OnPointHit(hitPosition, -transform.right, collision.name);
        }
    }

    protected override void OnObjectHit(Collider2D collision)
    {
        if (Exploded)
            return;

        Explode();
    }

    protected override void OnMaxDistanceReach()
    {
        if (Exploded)
            return;

        Explode();

        base.OnMaxDistanceReach();
    }

    private void Explode()
    {
        var hitPosition = transform.position;

        var explosion = Instantiate(ExplosionPrefab, hitPosition, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * ExplosionSpriteSize;

        var hitObjects = Physics2D.OverlapCircleAll(hitPosition, ExplosionMinDamageRadius, TargetLayerMask);

        var enemiesHit = hitObjects.Select(x => new { target = x.GetComponentInParent<IPlayerTarget>(), collider = x }).Where(x => x.target != null).ToList();
        var playersHit = hitObjects.Select(x => new { target = x.GetComponentInParent<IEnemyTarget>(), collider = x }).Where(x => x.target != null).ToList();

        foreach (var hit in enemiesHit)
        {
            IPlayerTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var enemyHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(enemyHitPoint, transform.position);

            if (distance <= ExplosionMinDamageRadius)
            {
                if (target.IsAlive)
                {
                    IsTargetHit = true;

                    int targetId = target.gameObject.GetInstanceID();

                    if (!hitTargetsIds.Contains(targetId))
                    {
                        hitTargetsIds.Add(targetId);
                        if (target is IKnockBackable knockBackable)
                        {
                            Vector3 direction = target.transform.position - transform.position;
                            knockBackable.TakeKnockBack(explosionPushForce, direction.normalized);
                        }
                    }

                    LastEnemyHit = target;

                }

                var clampedDistance = Mathf.Clamp(distance, ExplosionMaxDamageRadius, ExplosionMinDamageRadius);
                var percentage = (clampedDistance - ExplosionMaxDamageRadius) / (ExplosionMinDamageRadius - ExplosionMaxDamageRadius);

                Damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

                target.TakeDamage(Damage, HeadshotMultiplier, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name, PlayerOwner);
                target.OnPointHit(enemyHitPoint, -transform.right, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name);
            }
        }

        foreach (var hit in playersHit)
        {
            IEnemyTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var playerHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(playerHitPoint, transform.position);

            if (distance <= ExplosionMinDamageRadius)
            {
                if (target.IsAlive)
                {
                    IsTargetHit = true;
                    int targetId = target.gameObject.GetInstanceID();

                    if (hitTargetsIds.Contains(targetId))
                        continue;
                    else
                        hitTargetsIds.Add(targetId);

                    LastPlayerHit = target;

                    if (target is IKnockBackable knockBackable)
                    {
                        Vector3 direction = target.transform.position - transform.position;
                        knockBackable.TakeKnockBack(explosionPushForce, direction.normalized);
                    }
                }

                var clampedDistance = Mathf.Clamp(distance, ExplosionMaxDamageRadius, ExplosionMinDamageRadius);
                var percentage = (clampedDistance - ExplosionMaxDamageRadius) / (ExplosionMinDamageRadius - ExplosionMaxDamageRadius);

                Damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

                target.TakeDamage(Damage, HeadshotMultiplier, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name, null, selfDamage: true);
                target.OnPointHit(playerHitPoint, -transform.right, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name);
            }
        }

        Sprite.enabled = false;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;
        Collider.enabled = false;
        Exploded = true;
        StartCoroutine(KillSelfDelayed(2));
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, ExplosionMinDamageRadius);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, ExplosionMaxDamageRadius);
    //}
}
